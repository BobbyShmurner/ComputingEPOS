from typing import Optional
from .doc_element import IDocElement

class Paragraph(IDocElement):
	def __init__(self, text: str):
		super().__init__()
		self.text = text

	def serialize(self) -> dict:
		return {
			"type": self.get_type(),
			"text": self.text
		}

	@classmethod
	def deserialize(cls, data) -> 'Paragraph':
		text = data["text"]
		return cls(text)
	
	def doc_gen(self, doc):
		doc.add_paragraph(self.text)

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