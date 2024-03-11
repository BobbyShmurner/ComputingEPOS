from typing import Optional
from .doc_element import IDocElement

from docx.shared import RGBColor
from docx.shared import Pt

class Paragraph(IDocElement):
	def __init__(self, text: str, font: str = None, font_size: Optional[int] = None, font_color: Optional[list[int]] = None):
		super().__init__()

		self.text = text
		self.font = font
		self.font_size = font_size
		self.font_color = font_color
		

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
	
	def doc_gen(self, doc):
		run = doc.add_paragraph().add_run(self.text)

		if self.font:
			run.font.name = self.font

		if self.font_size:
			run.font.size = Pt(self.font_size)

		if self.font_color:
			run.font.color.rgb = RGBColor(*self.font_color)
		

	@classmethod
	def wizard(cls) -> Optional['Paragraph']:
		cls.cls()
		text = input("Enter paragraph text: ")

		if text == "":
			return None

		return Paragraph(text)
	
	def edit(self):
		self.cls()
		self.text = input(f"Current: {self.text}\nPlease enter new text:\n\n>> ")

	def __str__(self) -> str:
		return f"Paragraph ({self.text})"
	
	def __repr__(self) -> str:
		return f"Paragraph({self.text})"