

import shutil
import os

class Context:
	_instance = None

	def __new__(cls, project_path, *args, **kwargs):
		if not cls._instance:
			cls._instance = super().__new__(cls, *args, **kwargs)

		return cls._instance

	def __init__(self, project_path: str, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.project_path = os.path.abspath(project_path)

	def copy_content(self, path: str, content_path: str) -> str:
		"""
		Copies a file on disc to the project folder.

		Parameters:
		path: The path of the file to be copied.
		content_path: The relative path within the project where the file should be copied.

		Returns:
		str: the absolute path of the content.
		"""

		abs_content_path = os.path.join(self.project_path, content_path)
		abs_content_dir = os.path.dirname(abs_content_path)

		os.makedirs(abs_content_dir, exist_ok=True)
		shutil.copy(path, abs_content_path)

		return self.get_content_path(content_path)

	def get_content_path(self, content_path: str = "") -> str:
		"""Returns the absolute path of the content"""

		return os.path.join(self.project_path, content_path)
	
	def assign_document(self, doc):
		self.doc = doc

	def save_document(self):
		try:
			self.doc.serialize_to_disk()
		except Exception as e:
			pass
		