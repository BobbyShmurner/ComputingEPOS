from .paragraph import Paragraph
from .picture import Picture
from .test import Test
from .test_list import TestList
from .screenshot import Screenshot

Paragraph.register_type()
Picture.register_type()
Screenshot.register_type()
Test.register_type()
TestList.register_type()

__all__ = ['Paragraph', 'Picture', 'Screenshot', 'Test', 'TestList']