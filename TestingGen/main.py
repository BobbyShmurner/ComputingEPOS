import sys
import os

from src.DocElements.document import Document
from src.context import Context

from src.cancelable_input import CancelableInput

def cls():
	os.system("cls")

def main():
	if sys.argv[1:]:
		path = sys.argv[1]
	else:
		path = input("Enter the path of the project:\n\n>> ")

	Context(path)

	if os.path.exists(os.path.join(path, "Document.json")):
		doc = Document.deserialize_from_disk()
		Context._instance.assign_document(doc)
		
		doc.edit()
	else:
		doc = Document.wizard(assign_context = True)

	cls()
	
	print("Saving To Disk...")
	doc.serialize_to_disk()

	print("Loading Document...")
	doc = Document.deserialize_from_disk()

	print("Generating Document...")
	doc.doc_gen()

if __name__ == "__main__":
	if os.name != "nt":
		print("This script only works on Windows")
		exit(1)

	main()