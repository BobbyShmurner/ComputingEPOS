from typing import Optional

from src.cancelable_input import CancelableInput
from src.path_tree import PathTree
from .doc_element import IDocElement

from docx.document import Document as DocumentType

class Spacing(IDocElement):
	def __init__(self, lines: int):
		super().__init__()

		self.lines = lines

		super().save_document()
		

	def serialize(self) -> dict:
		data = {
			"type": self.get_type(),
			"lines": self.lines
		}

		return data

	@classmethod
	def deserialize(cls, data) -> 'Spacing':
		lines = data["lines"]

		return cls(lines)
	
	def doc_gen(self, doc: DocumentType):
		doc.add_paragraph("\n" * self.lines)

	@classmethod
	def wizard(cls) -> Optional['Spacing']:
		return Spacing(1)
	
	def edit(self):
		with PathTree("Spacing"):
			PathTree.cls()

			out = CancelableInput.input("Lines of space: ", self.lines)
			if out == None:
				return
			
			try:
				self.lines = int(out)
			except:
				return

	def __str__(self) -> str:
		return f"Spacing ({self.lines})"
	
	def __repr__(self) -> str:
		return f"Spacing({repr(self.lines)})"