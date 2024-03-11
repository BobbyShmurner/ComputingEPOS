from .paragraph import Paragraph
from .picture import Picture
from .screenshot import Screenshot
from .spacing import Spacing
from .test import Test
from .test_list import TestList

Paragraph.register_type()
Picture.register_type()
Screenshot.register_type()
Spacing.register_type()
Test.register_type()
TestList.register_type()

__all__ = ['Paragraph', 'Picture', 'Screenshot', 'Spacing', 'Test', 'TestList']