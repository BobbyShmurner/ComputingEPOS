from docx.document import Document as DocumentType
from abc import abstractmethod
from typing import Optional
import os

from src.context import Context

class IDocElement:
	type_names = {}

	def __init__(self) -> None:
		self.context = Context._instance
		
	@classmethod
	def register_type(cls):
		IDocElement.type_names[cls.__name__] = cls

	@classmethod
	def type_name_to_cls(cls, type_name: str) -> 'IDocElement':
		return cls.type_names[type_name]

	@abstractmethod
	def serialize(self) -> dict:
		pass
	
	@staticmethod
	def deserialize(data) -> 'IDocElement':
		type_name = data["type"]
		cls = IDocElement.type_names[type_name]

		return cls.deserialize(data)

	@abstractmethod
	def doc_gen(self, doc: DocumentType):
		pass

	@classmethod
	@abstractmethod
	def wizard(cls) -> Optional['IDocElement']:
		pass

	@abstractmethod
	def edit(self):
		pass

	def get_type(self) -> str:
		return self.__class__.__name__