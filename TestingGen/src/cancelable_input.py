import msvcrt
import re
import io
from typing import Optional
import sys
import ctypes
from ctypes import wintypes

import win32console

from src.path_tree import PathTree

class CancelableInput:
	__stdin = win32console.GetStdHandle(win32console.STD_INPUT_HANDLE)

	@classmethod
	def input(cls, prompt: Optional[str], ans: Optional[str] = None) -> Optional[str]:
		keys = []

		for c in str(ans):
			evt = win32console.PyINPUT_RECORDType(win32console.KEY_EVENT)
			evt.Char = c
			evt.RepeatCount = 1
			evt.KeyDown = True

			keys.append(evt)

		cls.__stdin.WriteConsoleInput(keys)

		try:
			return input(prompt)
		except KeyboardInterrupt:
			return None
		

# class CancelableInput:
# 	@staticmethod
# 	# https://stackoverflow.com/a/69582478
# 	def get_cursor_pos() -> tuple[int, int]:
# 		OldStdinMode = ctypes.wintypes.DWORD()
# 		OldStdoutMode = ctypes.wintypes.DWORD()
# 		kernel32 = ctypes.windll.kernel32
# 		kernel32.GetConsoleMode(kernel32.GetStdHandle(-10), ctypes.byref(OldStdinMode))
# 		kernel32.SetConsoleMode(kernel32.GetStdHandle(-10), 0)
# 		kernel32.GetConsoleMode(kernel32.GetStdHandle(-11), ctypes.byref(OldStdoutMode))
# 		kernel32.SetConsoleMode(kernel32.GetStdHandle(-11), 7)

# 		try:
# 			_ = ""
# 			sys.stdout.write("\x1b[6n")
# 			sys.stdout.flush()
# 			while not (_ := _ + sys.stdin.read(1)).endswith('R'):
# 				True
# 			res = re.match(r".*\[(?P<y>\d*);(?P<x>\d*)R", _)
# 		finally:
# 			kernel32.SetConsoleMode(kernel32.GetStdHandle(-10), OldStdinMode)
# 			kernel32.SetConsoleMode(kernel32.GetStdHandle(-11), OldStdoutMode)

# 		if res:
# 			return (int(res.group("x")), int(res.group("y")))
		
# 		return (-1, -1)	
	
# 	@staticmethod
# 	def set_cursor_pos(x, y):
# 		sys.stdout.write(f"\x1b[{y};{x}H")
# 		sys.stdout.flush()

# 	@staticmethod
# 	def move_cursor_forward():
# 		print(f"\x1b[C", end="", flush=True)

# 	@staticmethod
# 	def clear_screen(n: int = 0):
# 		print(f"\x1b[{n}J", end="", flush=True)

# 	@staticmethod
# 	def get_screen_size() -> tuple[int, int]:
# 		pos = CancelableInput.get_cursor_pos()
# 		CancelableInput.set_cursor_pos(9999, 9999)
# 		size = CancelableInput.get_cursor_pos()
# 		CancelableInput.set_cursor_pos(*pos)

# 		return size

# 	@staticmethod
# 	def goto_end_of_line():
# 		screen_width, screen_height = CancelableInput.get_screen_size()
# 		print("\x1b[1F", end='', flush=True)
# 		_, y = CancelableInput.get_cursor_pos()
# 		CancelableInput.set_cursor_pos(screen_width, y)


# 	@staticmethod
# 	def input(prompt: Optional[str], ans: Optional[str] = None) -> Optional[str]:
# 		CancelableInput.set_cursor_pos(*CancelableInput.get_cursor_pos())
# 		index = len(ans) if ans else 0

# 		if prompt: print(prompt, end="", flush=True)

# 		if ans:
# 			ans = str(ans)
# 			print(ans, end="", flush=True)
# 		else:
# 			ans = ""

# 		while True:
# 			c = msvcrt.getch()

# 			match c:
# 				case b"\r":
# 					print()
# 					return ans
# 				case b"\x08": # Backspace
# 					if len(ans) > 0 and index > 0:
# 						ans = ans[:index-1] + ans[index:]
# 						index -= 1

# 						old_x, old_y = CancelableInput.get_cursor_pos()
# 						if old_x == 1:
# 							CancelableInput.goto_end_of_line()
# 						else:
# 							print("\x08", end="", flush=True)
							
# 						cursor_pos = CancelableInput.get_cursor_pos()

# 						CancelableInput.clear_screen()
# 						print(ans[index:], end="", flush=True)
# 						CancelableInput.set_cursor_pos(*cursor_pos)
# 				case b"\x03": # Ctrl+C
# 					print()
# 					return None
# 				case b"\x1b": # Escape
# 					print()
# 					return None
# 				case b"\xe0": # Special Keys
# 					c = msvcrt.getch()
					
# 					match c:
# 						case b"K": # Left Arrow
# 							if index <= 0:
# 								continue

# 							index = index - 1
# 							print(f"\x1b[D", end="", flush=True)
# 						case b"M": # Right Arrow
# 							if index >= len(ans):
# 								continue

# 							index = index + 1
# 							CancelableInput.move_cursor_forward()
# 						case b"H": # Up Arrow
# 							pass
# 						case b"P": # Down Arrow
# 							pass
# 				case _:
# 					try:
# 						c_utf8 = c.decode("utf-8")

# 						if index == len(ans):
# 							ans += c_utf8
# 							print(c_utf8, end="", flush=True)
# 						else:
# 							old_pos = CancelableInput.get_cursor_pos()

# 							ans = ans[:index] + c_utf8 + ans[index:]
# 							print(c_utf8 + ans[index+1:], end="", flush=True)

# 							CancelableInput.set_cursor_pos(*old_pos)
# 							CancelableInput.move_cursor_forward()

# 						index += 1
# 					except:
# 						pass

	@staticmethod
	def input_chain(prompts: list[str]) -> Optional[list[str]]:
		answers = ["" for _ in prompts]
		i = 0

		while i < len(prompts):
			PathTree.cls()
			for j in range(i):
				print(f"{prompts[j]}{answers[j]}")
			
			res = CancelableInput.input(prompts[i], answers[i])

			if res == None:
				i -= 1
				if i < 0:
					return None
				
				continue
			
			answers[i] = res
			i += 1

		return answers
		