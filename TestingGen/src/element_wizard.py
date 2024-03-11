import msvcrt
import os
from typing import Optional

from src.DocElements.doc_element import IDocElement

class ElementWizard:
	status: str = "Please Choose an Option..."

	@staticmethod
	def cls():
		os.system('cls')
		
	@classmethod
	def wizard(cls, elements: list[IDocElement], can_add: bool = True, can_edit: bool = True, can_remove: bool = True, allowed_types_to_add: list[str] = None, ignored_types_to_add: list[str] = None, callback: Optional[callable] = None):
		options = []
		if can_add: options.append("Add Element")
		if can_edit: options.append("Edit Element")
		if can_remove: options.append("Remove Element")

		cls.status = "Please Choose an Option..."

		while True:
			index = cls.selection_wizard(options, cancel_text="Save and Quit")

			if index == -1:
				return

			functions = []
			if can_add: functions.append(lambda e: cls.add_wizard(e, allowed_types_to_add, ignored_types_to_add))
			if can_edit: functions.append(cls.edit_wizard)
			if can_remove: functions.append(cls.remove_wizard)

			functions[index](elements)
			callback()


	@classmethod
	def add_wizard(cls, elements: list[IDocElement], allowed_types: list[str] = None, ignored_types: list[str] = None):
		if not allowed_types:
			allowed_types = [str(k) for k in IDocElement.type_names.keys()]

		if ignored_types:
			allowed_types = [t for t in allowed_types if t not in ignored_types]

		index = cls.selection_wizard(allowed_types, "Please Choose an Element to Add:")
		if index == -1:
			cls.status = "Cancelled"
			return

		e = IDocElement.type_name_to_cls(allowed_types[index]).wizard()
		
		if e:
			elements.append(e)
			cls.status = f"Added {e.get_type()}"
		else:
			cls.status = "Cancelled"

	@classmethod
	def edit_wizard(cls, elements: list[IDocElement]):
		index = cls.selection_wizard(elements, "Please select an element to Edit:")
		if index == -1:
			cls.status = "Cancelled"
			return

		e = elements[index]
		e.edit()

		cls.status = f"Edited {e.get_type()}"

	@classmethod
	def remove_wizard(cls, elements: list[IDocElement]):
		index = cls.selection_wizard(elements, "Please select an element to Remove:")
		
		if index == -1:
			cls.status = "Cancelled"
			return

		e = elements.pop(index)
		cls.status = f"Removed {e.get_type()}"

	@classmethod
	def selection_wizard(cls, items: list[str], status: Optional[str] = None, cancel_text: str = "Cancel") -> int:
		index = 0

		if status:
			cls.status = status
		
		while True:
			cls.cls()
			print(f"{cls.status}\n")

			options = items.copy()
			options.append(cancel_text)

			for i, element in enumerate(options):
				prefix = "-" if i != index else ">"
				print(f"{prefix} [{i + 1}] {element}")

			c = ord(msvcrt.getch())

			match c:
				case num  if num >= 49 and num <= 57: # 1 - 9
					index = num - 49
					break
				case 113: # q
					return -1
				case 27: # Esc
					return -1
				case 13: # Enter
					break
				case 224: # Special Key
					c = ord(msvcrt.getch())
					match c:
						case 72: # Up Arrow
							index = max(0, index - 1)
						case 80: # Down Arrow
							index = min(len(items), index + 1)

		if index == len(items):
			cls.status = "Cancled"
			return -1

		return index