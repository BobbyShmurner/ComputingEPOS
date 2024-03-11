from typing import Optional
from .doc_element import IDocElement

from docx.shared import Pt
from docx.shared import RGBColor

class Test(IDocElement):
	def __init__(self, title: str, passed: bool, expected_output: str):
		super().__init__()

		self.title = title
		self.passed = passed
		self.expected_output = expected_output
		

	def serialize(self) -> dict:
		return {
			"type": self.get_type(),
			"title": self.title,
			"passed": self.passed,
			"expected_output": self.expected_output
		}

	@classmethod
	def deserialize(cls, data) -> 'Test':
		title = data["title"]
		passed = data["passed"]
		expected_output = data["expected_output"]

		return cls(title, passed, expected_output)
	
	def doc_gen(self, doc):
		title_run = doc.add_paragraph().add_run(self.title)
		title_run.font.size = Pt(14)

		doc.add_paragraph(f"Expected Output: {self.expected_output}")

		passed_run = doc.add_paragraph().add_run("Test: PASSED" if self.passed else "Test: FAILED")
		passed_run.font.color.rgb = RGBColor(0, 255, 0) if self.passed else RGBColor(255, 0, 0)

	@classmethod
	def wizard(cls) -> Optional['Test']:
		cls.cls()
		# text = input("Enter paragraph text: ")

		# if text == "":
		# 	return None

		# return Paragraph(text)
	
	def edit(self):
		self.cls()
		# self.text = input(f"Current: {self.text}\nPlease enter new text:\n\n>> ")

	def __str__(self) -> str:
		pass
		# return f"Paragraph ({self.text})"
	
	def __repr__(self) -> str:
		pass
		# return f"Paragraph({self.text})"