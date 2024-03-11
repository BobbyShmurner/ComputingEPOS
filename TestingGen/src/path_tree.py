import os

class PathTree:
	path: list[str] = []

	def __init__(self, path_item: str):
		self.path_item = path_item

	def __enter__(self):
		PathTree.path.append(self.path_item)

	def __exit__(self, exc_type, exc_value, traceback):
		PathTree.path.pop()

	@classmethod
	def cls(cls, print_path: bool = True):
		os.system("cls" if os.name == "nt" else "clear")

		if print_path:
			cls.print_path()

	@classmethod
	def print_path(cls):
		print(" > ".join(cls.path), end="\n\n")
	