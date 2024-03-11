import sys
import os

from src.DocElements.document import Document
from src.context import Context

def cls():
	os.system('cls')

def main():
	if sys.argv[1:]:
		path = sys.argv[1]
	else:
		path = input("Enter the path of the project:\n\n>> ")

	Context(path)

	if os.path.exists(os.path.join(path, "Document.json")):
		doc = Document.deserialize_from_disk()
		doc.edit()
	else:
		doc = Document.wizard()

	doc.doc_gen()

if __name__ == "__main__":
	if os.name != "nt":
		print("This script only works on Windows")
		exit(1)

	main()