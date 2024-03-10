import os
import json
import time
import msvcrt
from pathlib import Path
from typing import Optional

from src.context import Context
from .doc_element import IDocElement
from src.DocElements import Picture, Paragraph

import docx
from docx.document import Document as DocumentType

class Document(IDocElement):
	def __init__(self, elements: list[IDocElement] = []):
		super().__init__()
		self.elements = elements

		self.doc: DocumentType = docx.Document()	
		self.doc.sections[0].left_margin = self.doc.sections[0].page_width * 0.1
		self.doc.styles['Normal'].font.name = "Arial"

	def add_element(self, element: 'IDocElement'):
		self.elements.append(element)

	def remove_element(self, index: int):
		self.elements.pop(index)

	def edit_element(self, index: int):
		self.elements[index].edit()

	def serialize(self) -> dict:
		return {
			"type": self.get_type(),
			"elements": [e.serialize() for e in self.elements]
		}

	def serialize_to_disk(self):
		path = self.context.get_content_path("Document.json")
		os.makedirs(os.path.dirname(path), exist_ok=True)

		with open(path, "w+") as file:
			file.write(json.dumps(self.serialize(), indent=4))

	@classmethod
	def deserialize_from_disk(cls) -> 'Document':
		path = Context._instance.get_content_path("Document.json")

		with open(path, "r") as file:
			data = json.loads(file.read())

		return cls.deserialize(data)

	@classmethod
	def deserialize(cls, data) -> 'Document':
		elements_data = data["elements"]
		elements = []
		
		for data in elements_data:
			elements.append(IDocElement.deserialize(data))

		return cls(elements)
	
	def doc_gen(self, path: str = "out.docx"):
		for element in self.elements:
			element.doc_gen(self.doc)
			
		didPrint = False

		while True:
			try:
				self.doc.save(path)
				break
			except PermissionError:
				if not didPrint:
					self.cls()
					print("Please close the document in order to save it...")
					didPrint = True
				
				time.sleep(0.5)

		self.cls()
		print(f"Document saved to \"{path}\"")

		self.doc.save(path)

	@classmethod
	def wizard(cls) -> Optional['Document']:
		cls.cls()
		doc = cls()

		doc.edit()

		return doc
	
	def edit(self):
		status = "Please Choose an Option..."

		while True:
			self.cls()
			print(status, end="\n\n")
			print("[1] - Add Element\n[2] - Edit Element\n[3] - Remove Element\n[q] - Save and Quit")
			c = msvcrt.getch()

			match c:
				case b'q':
					break
				case b'1':
					self.add_wizard()
				case b'2':
					self.edit_wizard()
				case b'3':
					self.remove_wizard()

			self.serialize_to_disk()

	def add_wizard(self):
		status = "Please Choose an Option..."

		while True:
			self.cls()
			print(status, end="\n\n")
			print("[1] - Paragraph\n[2] - Picture\n[3] - Screenshot\n[q] - Cancel")
			c = msvcrt.getch()

			match c:
				case b'q':
					break
				case b'1':
					para = Paragraph.wizard()

					if para:
						self.add_element(para)
						status = "Paragraph Added!"
					else:
						status = "Cancelled"

					self.serialize_to_disk()
				case b'2':	
					img = Picture.wizard()
					
					if img:
						self.add_element(img)
						status = "Picture Added!"
					else:
						status = "Cancelled"

					self.serialize_to_disk()
				case b'3':
					img = Picture.screenshot_wizard()
					
					if img:
						self.add_element(img)
						status = "Picture Added!"
					else:
						status = "Cancelled"

					self.serialize_to_disk()

	def edit_wizard(self):
		index = self.selection_wizard("Please select an element to edit:")

		if index != -1:
			self.edit_element(index)

		self.serialize_to_disk()

	def remove_wizard(self):
		index = self.selection_wizard("Please select an element to remove:")

		if index != -1:
			self.remove_element(index)

		self.serialize_to_disk()

	def selection_wizard(self, title) -> int:
		index = 0
		
		while True:
			self.cls()
			print(f"{title}\n")

			options = self.elements.copy()
			options.append("Cancel")

			for i, element in enumerate(options):
				prefix = "-" if i != index else ">"
				print(f"{prefix} {element}")

			c = ord(msvcrt.getch())

			match c:
				case 113: # q
					return -1
				case 13: # Enter
					if index == len(self.elements):
						return -1

					return index
				case 224: # Special Key
					c = ord(msvcrt.getch())
					match c:
						case 72: # Up Arrow
							index = max(0, index - 1)
						case 80: # Down Arrow
							index = min(len(self.elements), index + 1)

	def __str__(self) -> str:
		return f"Document ({Path(self.context.project_path).parent.name})"
	
	def __repr__(self) -> str:
		return f"Document([{', '.join([repr(e) for e in self.elements])}])"