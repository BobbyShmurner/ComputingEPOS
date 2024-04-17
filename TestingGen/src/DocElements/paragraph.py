from typing import Optional

from src.cancelable_input import CancelableInput
from src.path_tree import PathTree
from .doc_element import IDocElement

from docx.document import Document as DocumentType
from docx.shared import RGBColor
from docx.shared import Pt

class Paragraph(IDocElement):
	def __init__(self, text: str, font: str = None, font_size: Optional[int] = None, font_color: Optional[list[int]] = None):
		super().__init__()

		self.text = text
		self.font = font
		self.font_size = font_size
		self.font_color = font_color

		super().save_document()
		

	def serialize(self) -> dict:
		data = {
			"type": self.get_type(),
			"text": self.text
		}

		if (self.font):
			data["font"] = self.font

		if (self.font_size):
			data["font_size"] = self.font_size

		if (self.font_color):
			data["font_color"] = self.font_color

		return data

	@classmethod
	def deserialize(cls, data) -> 'Paragraph':
		text = data["text"]
		font = data["font"] if "font" in data else None
		font_size = data["font_size"] if "font_size" in data else None
		font_color = data["font_color"] if "font_color" in data else None

		return cls(text, font, font_size, font_color)
	
	def doc_gen(self, doc: DocumentType):
		run = doc.add_paragraph().add_run(self.text)

		if self.font:
			run.font.name = self.font

		if self.font_size:
			run.font.size = Pt(self.font_size)

		if self.font_color:
			run.font.color.rgb = RGBColor(*self.font_color)
		

	@classmethod
	def wizard(cls) -> Optional['Paragraph']:
		with PathTree("Paragraph"):
			PathTree.cls()

			out = CancelableInput.input("Enter paragraph text: ")
			if not out:
				return None

			return Paragraph(out)
	
	def edit(self):
		with PathTree("Paragraph"):
			PathTree.cls()

			out = CancelableInput.input("Paragraph: ", self.text)
			if out == None:
				return
			
			self.text = out

	def __str__(self) -> str:
		return f"Paragraph ({self.text})"
	
	def __repr__(self) -> str:
		string = f"Paragraph({repr(self.text)}"

		if self.font: string += ", font=" + repr(self.font)
		if self.font_size: string += ", font_size=" + repr(self.font_size)
		if self.font_color: string += ", font_color=" + repr(self.font_color)

		return string + ")"