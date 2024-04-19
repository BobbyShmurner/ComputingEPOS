import sys
import os

import docx
from docx.text.paragraph import Paragraph
from docx.text.run import Run
from docx.document import Document as DocumentType

from src.DocElements.widget import Widget
from src.DocElements.document import Document
from src.context import Context

from src.cancelable_input import CancelableInput

def cls():
	os.system("cls")

def main():
	if len(sys.argv) > 2 and sys.argv[1] == "--covers":
		doc: DocumentType = docx.Document(sys.argv[2])

		with open(sys.argv[2] + ".xml", 'w+') as file:
			file.write(doc.element.xml)

		runs_with_rpbs = [
			Run(r, doc)
			for r in doc.element.xpath("./w:body/w:p/w:r[w:lastRenderedPageBreak]")
		]

		Context(r"C:\dev\School\ComputingEPOS\TestingGen\docs\Debug")
		widget = Widget("PageBG")

		for r in runs_with_rpbs:
			# r._element.getparent().insert(0, r._element)
   			widget.doc_gen(doc, r)

		with open(sys.argv[2] + "_new.xml", 'w+') as file:
			file.write(doc.element.xml)

		doc.save(sys.argv[2].replace(".docx", "_new.docx"))
		sys.exit(0)

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