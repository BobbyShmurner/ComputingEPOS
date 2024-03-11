import os
import json
import time
import msvcrt
from pathlib import Path
from typing import Optional

from src.context import Context
from src.element_wizard import ElementWizard
from .doc_element import IDocElement
from src.DocElements import Picture, Paragraph

import docx
from docx.shared import Pt, RGBColor
from docx.document import Document as DocumentType

class Document(IDocElement):
	def __init__(self, elements: list[IDocElement] = [], font: Optional[str] = None, font_size: Optional[int] = None):
		super().__init__()

		self.elements = elements
		self.font = font
		self.font_size = font_size

	def add_element(self, element: 'IDocElement'):
		self.elements.append(element)

	def serialize(self) -> dict:
		data = {
			"type": self.get_type(),
			"elements": [e.serialize() for e in self.elements]
		}

		if self.font:
			data["font"] = self.font

		if self.font_size:
			data["font_size"] = self.font_size

		return data

	@classmethod
	def deserialize(cls, data) -> 'Document':
		elements_data = data["elements"]
		elements = []
		
		for data in elements_data:
			elements.append(IDocElement.deserialize(data))

		font = data["font"] if "font" in data else None
		font_size = data["font_size"] if "font_size" in data else None

		return cls(elements, font, font_size)
	
	def serialize_to_disk(self):
		path = self.context.get_content_path("Document.json")
		os.makedirs(os.path.dirname(path), exist_ok=True)

		back = ""
		with open(path, "r+") as file:
			back = file.read()

		try:
			with open(path, "w+") as file:
				file.write(json.dumps(self.serialize(), indent=4))
		except:
			with open(path, "w+") as file:
				file.write(back)

			raise	

	@classmethod
	def deserialize_from_disk(cls) -> 'Document':
		path = Context._instance.get_content_path("Document.json")

		with open(path, "r") as file:
			data = json.loads(file.read())

		return cls.deserialize(data)
	
	def doc_gen(self, path: str = "out.docx"):
		doc: DocumentType = docx.Document()	
		doc.sections[0].left_margin = doc.sections[0].page_width * 0.1

		doc.styles['Normal'].font.name = self.font if self.font else "Arial"

		if self.font_size:
			doc.styles['Normal'].font.size = Pt(self.font_size)

		for element in self.elements:
			element.doc_gen(doc)
			
		didPrint = False

		while True:
			try:
				doc.save(path)
				break
			except PermissionError:
				if not didPrint:
					self.cls()
					print("Please close the document in order to save it...")
					didPrint = True
				
				time.sleep(0.5)

		self.cls()
		print(f"Document saved to \"{path}\"")

		doc.save(path)

	@classmethod
	def wizard(cls) -> Optional['Document']:
		cls.cls()

		doc = cls()
		doc.edit()

		return doc
	
	def edit(self):
		ElementWizard.wizard(self.elements, callback=self.serialize_to_disk)

	def __str__(self) -> str:
		return f"Document ({Path(self.context.project_path).parent.name})"
	
	def __repr__(self) -> str:
		string = f"Document([{', '.join([repr(e) for e in self.elements])}]"

		if self.font: string += ", font=" + repr(self.font)
		if self.font_size: string += ", font_size=" + repr(self.font_size)

		return string + ")"