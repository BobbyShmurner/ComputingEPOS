import numpy as np
import win32gui
import time
import cv2
import os

from io import BytesIO
from PIL import Image, ImageGrab

from docx import Document
from docx.document import Document as DocumentType
import msvcrt

ENTER_KEY = 13
ESCAPE_KEY = 27
SPACE_KEY = 32
Q_KEY = 113

def edit_image(img: Image.Image, winName: str = "Edit Screenshot"):
	cv2.namedWindow(winName)

	def enumHandler(hwnd, _):
		try:
			if win32gui.GetWindowText(hwnd) == winName:
				win32gui.SetForegroundWindow(hwnd)
		except:
			return

	firstPass = True

	while True:
		cv2.imshow(winName, cv2.cvtColor(np.array(img), cv2.COLOR_RGBA2BGRA))

		if firstPass:
			win32gui.EnumWindows(enumHandler, None)
			firstPass = False

		c = cv2.waitKey(1) 
		isWindowClosed = cv2.getWindowProperty(winName, cv2.WND_PROP_VISIBLE) < 1

		if c == ENTER_KEY or c == ESCAPE_KEY or c == SPACE_KEY or c == Q_KEY or isWindowClosed:
			cv2.destroyAllWindows() 
			break

	return img

def cls():
	os.system('cls')

def img_to_stream(img: Image.Image) -> BytesIO:
	stream = BytesIO()
	img.save(stream, format="PNG")
	stream.seek(0)
	return stream

def save_doc(doc: DocumentType, path: str):
	didPrint = False

	while True:
		try:
			doc.save(path)
			break
		except PermissionError:
			if not didPrint:
				print("Please close the document in order to save it...")
				didPrint = True
			
			time.sleep(0.5)

	print(f"Document saved to \"{path}\"")

def grab_screenshot() -> Image.Image:
	old_img = ImageGrab.grabclipboard()
	img = old_img

	os.system("explorer ms-screenclip:")

	while True:
		img = ImageGrab.grabclipboard()

		if img != old_img and img != None:
			break

		time.sleep(0.1)

	return img

def create_doc() -> DocumentType:
	doc: DocumentType = Document()	
	doc.sections[0].left_margin = doc.sections[0].page_width * 0.1
	doc.styles['Normal'].font.name = "Arial"

	return doc

def main():
	status = "Please Choose an Option..."
	doc = create_doc()

	while True:
		cls()
		print(status, end="\n\n")
		print("[p] - Paragraph\n[s] - Screenshot\n[q] - Quit")
		c = msvcrt.getch()

		match c:
			case b'q':
				break
			case b's':	
				cls()
				print("Waiting for screenshot...")
				img = grab_screenshot()

				cls()
				print("Editing Screenshot...")
				img = edit_image(img)

				# Sometimes, the snipping tool popup can stop the window from being focused
				time.sleep(0.1)

				doc.add_picture(img_to_stream(img), width=doc.sections[0].page_width * 0.8)

				status = "Screenshot Added!"
			case b'p':
				cls()

				text = input("Enter paragraph text: ")
				doc.add_paragraph(text)

				status = "Paragraph Added!"

		cls()
		save_doc(doc, "out.docx")

if __name__ == "__main__":
	if os.name != "nt":
		print("This script only works on Windows")
		exit(1)

	main()