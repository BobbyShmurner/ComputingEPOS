import msvcrt
import os
import shutil
import PIL
import cv2
import time
import docx
import win32con
import win32gui
import numpy as np
import win32com.client
from io import BytesIO
from typing import Optional
from PIL import Image, ImageGrab
from docx.document import Document as DocumentType

from typing import TYPE_CHECKING

if TYPE_CHECKING:
	from docx.oxml.xmlchemy import BaseOxmlElement

from src.cancelable_input import CancelableInput
from src.element_wizard import ElementWizard
from src.path_tree import PathTree

from .doc_element import IDocElement
from ..context import Context

class Widget(IDocElement):
	def __init__(self, name: str):
		super().__init__()

		self.name = name

		super().save_document()

	def serialize(self) -> dict:
		data = {
			"type": self.get_type(),
			"name": self.name,
			
		}

		return data

	@classmethod
	def deserialize(cls, data: dict) -> 'Widget':
		name = data["name"]

		return cls(name)
	
	def doc_gen(self, doc: DocumentType):
		try:
			widget_doc: DocumentType = docx.Document(self.widget_path())

			with open(self.widget_path() + ".xml", 'w+') as file:
				file.write(widget_doc.element.body.xml)

			doc.add_paragraph().add_run().add_break(docx.enum.text.WD_BREAK.LINE)

			for element in widget_doc.element.body:
				element: BaseOxmlElement = element
				if element.tag.endswith("sectPr"): continue

				doc.element.body.insert(-1, element)
		except Exception as e:
			print("Error when generating document widget!\n\n")
			input(e)

	def widget_path(self) -> str:
		return os.path.join(self.widget_dir(), f"{self.name}.docx")
	
	@classmethod
	def widget_dir(cls) -> str:
		return Context._instance.get_content_path(f"Widgets/")

	@classmethod
	def wizard(cls) -> Optional['Widget']:
		with PathTree("Widget"):
			PathTree.cls()

			status = "Use existing Widget?"
			choices = ["Existing", "Create New"]

			widget_name: str = None

			while widget_name == None:
				choice = ElementWizard.selection_wizard(choices, status)
				match choice:
					case -1:
						return None
					case 0:
						
						widgets = [".".join(widget.split(".")[:-1]) for widget in os.listdir(cls.widget_dir())]
						status = "Please Select a Widget"

						
						while widget_name == None:
							choice = ElementWizard.selection_wizard(widgets, status)

							match choice:
								case -1:
									break
								case _:
									widget_name = widgets[choice]
					case 1:
						with PathTree("New"):
							answers = CancelableInput.input_chain("Widget Name: ", "Widget Path: ")
							if not answers: break

							widget_name = answers[0]
							widget_path = answers[1]

							if not os.path.exists(widget_path):
								status = "Path does not exist"
								break

							os.makedirs(cls.widget_dir(), exist_ok=True)
							shutil.copy(widget_path, os.path.join(cls.widget_dir(), f"{widget_name}.docx"))

		return cls(widget_name)

	def edit(self):
		with PathTree(str(self)):
			PathTree.cls()
			

	def __str__(self) -> str:
		return f"Widget (Name: {self.name})"
	
	def __repr__(self) -> str:
		return f"Widget({repr(self.name)})"