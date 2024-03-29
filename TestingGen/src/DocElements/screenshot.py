import time
from typing import Optional
from PIL import ImageGrab

from src.path_tree import PathTree

from .picture import Picture

class Screenshot(Picture):
	@classmethod
	def wizard(cls) -> Optional['Picture']:
		with PathTree("Screenshot"):
			PathTree.cls()
			print("Waiting for screenshot...")

			old_img = ImageGrab.grabclipboard()
			img = old_img

			# os.system("explorer ms-screenclip:")

			while True:
				try:
					img = ImageGrab.grabclipboard()

					if img != old_img and img != None:
						break

					time.sleep(0.1)
				except KeyboardInterrupt:
					return None

			return Picture.wizard(img, prompt_discard=True)