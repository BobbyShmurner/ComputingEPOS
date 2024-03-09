import time
import os

from io import BytesIO
from PIL import Image, ImageGrab

from docx import Document
from docx.document import Document as DocumentType
import msvcrt

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

def grab_screenshot() -> BytesIO:
	old_img = ImageGrab.grabclipboard()
	img = old_img

	print("Waiting for screenshot...")

	os.system("explorer ms-screenclip:")

	while True:
		img = ImageGrab.grabclipboard()

		if img != old_img and img != None:
			break

		time.sleep(0.1)

	return img_to_stream(img)

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

				img = grab_screenshot()
				doc.add_picture(img, width=doc.sections[0].page_width * 0.8)

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