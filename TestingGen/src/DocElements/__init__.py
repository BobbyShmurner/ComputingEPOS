from .paragraph import Paragraph
from .page_break import PageBreak
from .picture import Picture
from .screenshot import Screenshot
from .spacing import Spacing
from .test import Test
from .test_list import TestList
from .widget import Widget

Paragraph.register_type()
Picture.register_type()
Screenshot.register_type()
Test.register_type()
TestList.register_type()
Widget.register_type()
Spacing.register_type()
PageBreak.register_type()

__all__ = ['Paragraph', 'Picture', 'Screenshot', 'Test', 'TestList', 'Widget', 'Spacing', 'PageBreak']