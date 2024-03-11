import msvcrt
import os
from typing import Optional

from src.path_tree import PathTree

class CancelableInput:
	@staticmethod
	def input(prompt: Optional[str], ans: Optional[str] = None) -> Optional[str]:
		if prompt: print(prompt, end="", flush=True)

		if ans:
			ans = str(ans)
			print(ans, end="", flush=True)
		else:
			ans = ""

		while True:
			c = msvcrt.getch()

			match c:
				case b"\r":
					print()
					return ans
				case b"\x08": # Backspace
					if len(ans) > 0:
						ans = ans[:-1]
						print("\b \b", end="", flush=True)
				case b"\x03": # Ctrl+C
					print()
					return None
				case b"\x1b": # Escape
					print()
					return None
				case b"\xe0": # Special Keys
					msvcrt.getch()
					pass
				case _:
					try:
						ans += c.decode("utf-8")
						print(c.decode("utf-8"), end="", flush=True)
					except:
						pass

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
		