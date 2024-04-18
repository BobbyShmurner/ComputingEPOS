from typing import Optional

from .doc_element import IDocElement

from docx.document import Document as DocumentType

class PageBreak(IDocElement):
	def __init__(self):
		super().__init__()

		super().save_document()
		

	def serialize(self) -> dict:
		data = {
			"type": self.get_type()
		}

		return data

	@classmethod
	def deserialize(cls, data) -> 'PageBreak':
		return cls()
	
	def doc_gen(self, doc: DocumentType):
		doc.add_page_break()

	@classmethod
	def wizard(cls) -> Optional['PageBreak']:
		return PageBreak()
	
	def edit(self):
		return
	
	@classmethod
	def display_name(cls) -> str:
		return "Page Break"

	def __str__(self) -> str:
		return f"Page Break"
	
	def __repr__(self) -> str:
		return f"PageBreak()"