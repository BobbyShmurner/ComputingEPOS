import msvcrt
import os
from typing import Optional

from src.DocElements.doc_element import IDocElement
from src.path_tree import PathTree
class ElementWizard:
	status: str = "Please Choose an Option..."
		
	@classmethod
	def wizard(cls, elements: list[IDocElement], path_title: str, can_add: bool = True, can_edit: bool = True, can_remove: bool = True, can_reorder: bool = True, allowed_types_to_add: list[str] = None, ignored_types_to_add: list[str] = None, callback: Optional[callable] = None):
		with PathTree(path_title):
			options = []

			if can_add: options.append("Add Element")
			if can_edit: options.append("Edit Element")
			if can_remove: options.append("Remove Element")
			if can_reorder: options.append("Reorder Element")

			status = "Please Choose an Option..."

			while True:
				index = cls.selection_wizard(options, status=status, cancel_text="Save and Quit")

				if index == -1:
					return

				functions = []
				if can_add: functions.append(lambda e: cls.add_wizard(e, allowed_types_to_add, ignored_types_to_add))
				if can_edit: functions.append(cls.edit_wizard)
				if can_remove: functions.append(cls.remove_wizard)
				if can_reorder: functions.append(cls.reorder_wizard)

				functions[index](elements)
				if callback: callback()

	@classmethod
	def add_wizard(cls, elements: list[IDocElement], allowed_types: Optional[list[str]] = None, ignored_types: Optional[list[str]] = None, status: Optional[str] = None, cancel_option: str = "Cancel", path_name: str = "Add"):
		with PathTree(path_name):
			if not allowed_types:
				allowed_types = [str(k) for k in IDocElement.type_names.keys()]

			if ignored_types:
				allowed_types = [t for t in allowed_types if t not in ignored_types]

			choices = [IDocElement.type_names[t].display_name() for t in allowed_types]

			status = status if status else "Please Choose an Element to Add:"

			while True:
				index = cls.selection_wizard(choices, status=status, cancel_text=cancel_option)

				if index == -1:
					cls.status = "Cancelled"
					return

				e = IDocElement.type_name_to_cls(allowed_types[index]).wizard()
				
				if e:
					elements.append(e)
					status = f"Added {e.get_type()}"
				else:
					cls.status = None

	@classmethod
	def edit_wizard(cls, elements: list[IDocElement], status: Optional[str] = None, cancel_option: str = "Cancel", path_name: str = "Edit"):
		with PathTree(path_name):
			status = status if status else "Please Choose an Element to Edit:"

			while True:
				index = cls.selection_wizard(elements, status=status, cancel_text=cancel_option)

				if index == -1:
					cls.status = None
					return

				e = elements[index]
				e.edit()

				status = f"Edited {e.get_type()}"

	@classmethod
	def remove_wizard(cls, elements: list[IDocElement], status: Optional[str] = None, cancel_option: str = "Cancel", path_name: str = "Remove"):
		with PathTree(path_name):
			status = status if status else "Please Choose an Element to Remove:"

			while True:
				index = cls.selection_wizard(elements, status=status, cancel_text=cancel_option)
				
				if index == -1:
					cls.status = None
					return

				e = elements.pop(index)
				status = f"Removed {e.get_type()}"

	@classmethod
	def reorder_wizard(cls, elements: list[IDocElement], status: Optional[str] = None, cancel_option: str = "Cancel", path_name: str = "Reorder"):
		with PathTree(path_name):
			status = status if status else "Please Choose an Element to Move:"

			while True:
				index = cls.selection_wizard(elements, status=status, cancel_text=cancel_option)

				if index == -1:
					cls.status = None
					return
				
				with PathTree("Reordering..."):
					new_index = cls.selection_wizard(elements, status="Please Choose a New Position:", cancel_text="Cancel", reorder_index=index)

					if new_index == -1:
						status = "Canceled"
						continue
					
					e = elements.pop(index)
					elements.insert(new_index, e)

					status = f"Reordered {e.get_type()}"

	@classmethod
	def confirmation_wizard(cls, status: str, path_name: str = "Confirm") -> bool:
		with PathTree(path_name):
			index = cls.selection_wizard(["Yes"], status=status, cancel_text="No")
			
			if index == -1:
				return False
			
			return True

	@classmethod
	def selection_wizard(cls, items: list[str], status: Optional[str] = None, cancel_text: str = "Cancel", reorder_index: Optional[int] = 0) -> int:
		index = 0 if not reorder_index else reorder_index

		if status:
			cls.status = status
		
		while True:
			PathTree.cls()

			if not cls.status and status:
				cls.status = status

			print(f"{cls.status}\n")

			options = items.copy()
			options.append(cancel_text)

			for i in range(len(options)):
				if reorder_index:
					if index == len(options) - 1:
						option_i = i
					elif i == index:
						option_i = reorder_index
					elif index < reorder_index and i <= reorder_index and i > index:
						option_i = i - 1
					elif index > reorder_index and i >= reorder_index and i < index:
						option_i = i + 1
					else:
						option_i = i
				else:
					option_i = i

				prefix = "-" if i != index else ">"
				
				if reorder_index and i == index:
					prefix = "  " + prefix

				print(f"{prefix} [{option_i + 1 if option_i < 9 else ('-' if option_i < len(options) - 1 else 'q')}] {options[option_i]}")

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
							index = (index - 1) % len(options)
						case 80: # Down Arrow
							index = (index + 1) % len(options)

		if index == len(items):
			cls.status = "Cancled"
			return -1

		return index