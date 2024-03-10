from .paragraph import Paragraph
from .picture import Picture

Paragraph.register_type()
Picture.register_type()

__all__ = ['Paragraph', 'Picture']