import msvcrt
import os
import PIL
import cv2
import time
import win32con
import win32gui
import numpy as np
import win32clipboard
import win32com.client
from io import BytesIO
from typing import Optional
from PIL import Image, ImageGrab
from pynput.keyboard import Key, Listener

ENTER_KEY = 13
ESCAPE_KEY = 27
SPACE_KEY = 32
Q_KEY = 113
Y_KEY = 121
Z_KEY = 122

class StandalonePicture:
	def __init__(self, rects: list[list[int, int, int, int]] = [], shade: Optional[list[int, int, int, int]] = None):
		super().__init__()

		self.rects = rects
		self.shade = shade
	
	@classmethod
	def add_rects_to_img(cls, img: Image.Image, rects: list[list[int, int, int, int]]) -> Image.Image:
		img = np.array(img)
		
		for rect in rects:
			cv2.rectangle(img, (rect[0], rect[1]), (rect[2], rect[3]), (255, 0, 0, 255), 2)

		return Image.fromarray(img, "RGBA")
	
	@classmethod
	def add_shade_to_img(cls, img: Image.Image, shades: list[list[int, int, int, int]], alpha: float = 0.2) -> Image.Image:
		img = np.array(img)
		overlay = np.zeros_like(img)

		for shade in shades:
			cv2.rectangle(overlay, (shade[0], shade[1]), (shade[2], shade[3]), (255, 255, 255, 0), -1)

		dst = cv2.addWeighted(overlay, 1-alpha, img , alpha, 0)
		res = np.where(overlay==(255, 255, 255, 0), img, dst)

		return Image.fromarray(res, "RGBA")

	def get_img(self) -> Image.Image:
		img = None
		while not img:
			time.sleep(0.1)
			img = ImageGrab.grabclipboard()

		return img

	def edit(self):
		print("Waiting Picture...")

		win_name = "Edit Picture"
		base_img = self.get_img()
		
		click_point = (0, 0)
		lmb_down = False
		rmb_down = False

		rect_history = self.rects.copy()
		current_rect_pointer = len(rect_history) - 1
		shade = self.shade if self.shade else None

		def check_shade(shade: list[int, int, int, int]) -> bool:
			if not shade: return False
			return shade[0] != shade[2] and shade[1] != shade[3]

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
			nonlocal lmb_down, rmb_down, click_point, current_rect_pointer, base_img, rect_history, shade

			match event:
				case cv2.EVENT_LBUTTONDOWN:
					if rmb_down: return
					lmb_down = True

					rect_history = rect_history[:current_rect_pointer + 1]
					rect_history.append([x, y, x, y])
					current_rect_pointer += 1
				case cv2.EVENT_LBUTTONUP:
					lmb_down = False
				case cv2.EVENT_RBUTTONDOWN:
					if lmb_down: return
					rmb_down = True

					shade = [x, y, x, y]
				case cv2.EVENT_RBUTTONUP:
					rmb_down = False
				case cv2.EVENT_MOUSEMOVE:
					if lmb_down:
						rect_history[current_rect_pointer][2] = x
						rect_history[current_rect_pointer][3] = y
					elif rmb_down:
						shade[2] = x
						shade[3] = y

		cv2.namedWindow(win_name)
		cv2.setMouseCallback(win_name, onMouse)

		firstPass = True
		while True:
			display_img = base_img.copy()
			if check_shade(shade): display_img = self.add_shade_to_img(display_img, [shade])
			display_img = self.add_rects_to_img(display_img, rect_history[:current_rect_pointer + 1])

			cv2.imshow(win_name, cv2.cvtColor(np.array(display_img), cv2.COLOR_RGBA2BGRA))

			if firstPass:
				win32gui.EnumWindows(enumHandler, None)
				firstPass = False

			c = cv2.waitKey(1) 
			isWindowClosed = cv2.getWindowProperty(win_name, cv2.WND_PROP_VISIBLE) < 1

			if c == ENTER_KEY or c == ESCAPE_KEY or c == SPACE_KEY or c == Q_KEY or isWindowClosed:
				cv2.destroyAllWindows() 
				break

			if c == Z_KEY and not lmb_down and current_rect_pointer >= 0:
				current_rect_pointer -= 1

			if c == Y_KEY and not lmb_down and current_rect_pointer < len(rect_history) - 1:
				current_rect_pointer += 1

		self.rects = rect_history[:current_rect_pointer + 1]
		self.shade = shade if check_shade(shade) else None

		self.send_to_clipboard(display_img)

	def send_to_clipboard(self, image: Image.Image):
		output = BytesIO()
		image.convert("RGB").save(output, "BMP")
		data = output.getvalue()[14:]
		output.close()

		win32clipboard.OpenClipboard()
		win32clipboard.EmptyClipboard()
		win32clipboard.SetClipboardData(win32clipboard.CF_DIB, data)
		win32clipboard.CloseClipboard()



if __name__ == "__main__":
	def on_release(key):
		if key == Key.num_lock:
			return False

	while True:
		print("Press Num Lock To Start Editing")

		with Listener(on_release=on_release) as listener:
			listener.join()

		picture = StandalonePicture()
		picture.edit()