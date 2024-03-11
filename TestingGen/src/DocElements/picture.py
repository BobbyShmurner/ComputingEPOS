import msvcrt
import os
import PIL
import cv2
import time
import win32con
import win32gui
import numpy as np
import win32com.client
from io import BytesIO
from typing import Optional
from PIL import Image, ImageGrab
from docx.document import Document as DocumentType

from src.element_wizard import ElementWizard

from .doc_element import IDocElement
from ..context import Context

ENTER_KEY = 13
ESCAPE_KEY = 27
SPACE_KEY = 32
Q_KEY = 113
Y_KEY = 121
Z_KEY = 122

class Picture(IDocElement):
	def __init__(self, index: int, rects: list[list[int, int, int, int]] = []):
		super().__init__()

		self.index = index
		self.rects = rects

	def serialize(self) -> dict:
		return {
			"type": self.get_type(),
			"index": self.index,
			"rects": self.rects
		}

	@classmethod
	def deserialize(cls, data: dict) -> 'Picture':
		index = data["index"]
		rects = data["rects"] if "rects" in data else []

		return cls(index, rects)
	
	def doc_gen(self, doc: DocumentType):
		pic_path = self.context.get_content_path(f"Pictures/{self.index}.png")
		pic = self.add_rects_to_img(Image.open(pic_path), self.rects)

		pic_stream = BytesIO()
		pic.save(pic_stream, format="PNG")

		doc.add_picture(pic_stream, width=doc.sections[0].page_width * 0.8)

	def picture_path(self) -> str:
		return os.path.join(self.picture_dir(), f"{self.index}.png")
	
	@classmethod
	def picture_dir(cls) -> str:
		return Context._instance.get_content_path(f"Pictures/")

	@classmethod
	def next_index(cls) -> int:
		if not os.path.exists(Picture.picture_dir()):
			return 1
		
		files = os.listdir(Picture.picture_dir())

		index = 0
		for file in files:
			file_name = file.split(".")[0]
			if file_name.isnumeric():
				index = max(index, int(file_name))

		return index + 1

	@classmethod
	def screenshot_wizard(cls) -> Optional['Picture']:
		cls.cls()
		print("Waiting for screenshot...")

		old_img = ImageGrab.grabclipboard()
		img = old_img

		os.system("explorer ms-screenclip:")

		while True:
			img = ImageGrab.grabclipboard()

			if img != old_img and img != None:
				break

			time.sleep(0.1)

		return cls.wizard(img)

	@classmethod
	def wizard(cls, img: Image.Image = None) -> Optional['Picture']:
		cls.cls()

		status = "Where would you like to get the picture from?"
		choices = ["Clipboard", "File"]

		while img is None:
			choice = ElementWizard.selection_wizard(choices, status)

			match choice:
				case -1:
					return None
				case 0:
					img = ImageGrab.grabclipboard()
					status = "No Image found in clipboard"
				case 1:
					cls.cls()
					img = None

					try:
						path = input("Enter the path of the picture:\n\n>> ")

						if os.path.exists(path):
							img = Image.open(path)
							if img == None:
								status = "Invalid Image"
						else:
							status = "File does not exist"
					except PIL.UnidentifiedImageError:
						status = "Invalid Image"
						pass
					except KeyboardInterrupt:
						status = "Keyboard Interrupt"
						pass
					except PermissionError:
						status = "Permission Denied"
						pass

			if img:
				break

		index = cls.next_index()

		os.makedirs(cls.picture_dir(), exist_ok=True)
		img.save(os.path.join(cls.picture_dir(), f"{index}.png"))

		pic = Picture(int(index))
		pic.edit()

		return pic
	
	def add_rects_to_img(self, img: Image.Image, rects: list[list[int, int, int, int]]) -> Image.Image:
		img = np.array(img)
		
		for rect in rects:
			cv2.rectangle(img, (rect[0], rect[1]), (rect[2], rect[3]), (255, 0, 0, 255), 2)

		return Image.fromarray(img, "RGBA")

	def edit(self):
		self.cls()
		print("Editing Picture...")

		win_name = "Edit Picture"
		base_img = Image.open(self.picture_path())
		
		click_point = (0, 0)
		mouse_down = False

		rect_history = self.rects.copy()
		current_rect_pointer = len(rect_history) - 1

		def enumHandler(hwnd, _):
			try:
				if win32gui.GetWindowText(hwnd) == win_name:
					# For some odd reason, you have to press alt to bring the window to the front
					shell = win32com.client.Dispatch("WScript.Shell")
					shell.SendKeys('%')

					win32gui.ShowWindow(hwnd, win32con.SW_SHOWNORMAL)
					win32gui.SetForegroundWindow(hwnd)
			except Exception as e:
				print(e)
				return
			
		def onMouse(event, x, y, flags, param):
			nonlocal mouse_down, click_point, current_rect_pointer, base_img, rect_history

			match event:
				case cv2.EVENT_LBUTTONDOWN:
					rect_history = rect_history[:current_rect_pointer + 1]
					rect_history.append([x, y, x, y])
					current_rect_pointer += 1

					mouse_down = True
				case cv2.EVENT_LBUTTONUP:
					mouse_down = False
				case cv2.EVENT_MOUSEMOVE:
					if mouse_down:
						rect_history[current_rect_pointer][2] = x
						rect_history[current_rect_pointer][3] = y

		cv2.namedWindow(win_name)
		cv2.setMouseCallback(win_name, onMouse)

		firstPass = True
		while True:
			cv2.imshow(win_name, cv2.cvtColor(np.array(self.add_rects_to_img(base_img, rect_history[:current_rect_pointer + 1])), cv2.COLOR_RGBA2BGRA))

			if firstPass:
				win32gui.EnumWindows(enumHandler, None)
				firstPass = False

			c = cv2.waitKey(1) 
			isWindowClosed = cv2.getWindowProperty(win_name, cv2.WND_PROP_VISIBLE) < 1

			if c == ENTER_KEY or c == ESCAPE_KEY or c == SPACE_KEY or c == Q_KEY or isWindowClosed:
				cv2.destroyAllWindows() 
				break

			if c == Z_KEY and not mouse_down and current_rect_pointer >= 0:
				current_rect_pointer -= 1

			if c == Y_KEY and not mouse_down and current_rect_pointer < len(rect_history) - 1:
				current_rect_pointer += 1

		self.rects = rect_history[:current_rect_pointer + 1]

	def __str__(self) -> str:
		return f"Picture (Index: {self.index})"
	
	def __repr__(self) -> str:
		return f"Picture({repr(self.index)}, {repr(self.rects)})"