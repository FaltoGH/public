#include <windows.h>
#include <stdio.h>
#include <stdbool.h>

typedef int(__stdcall *pMessageBoxA) (HWND hWnd, LPCSTR lpText, LPCSTR lpCaption, UINT uType); //This is the 'type' of the MessageBoxA call.
pMessageBoxA RealMessageBoxA; //This will store a pointer to the original function.

void DetourIATptr(const char* function, void* newfunction, HMODULE module);

int __stdcall NewMessageBoxA(HWND hWnd, LPCSTR lpText, LPCSTR lpCaption, UINT uType) { //Our fake function
    printf("The String Sent to MessageBoxA Was : %s\n", lpText);
    return RealMessageBoxA(hWnd, lpText, lpCaption, uType); //Call the real function
}

int main(int argc, CHAR *argv[]) {
   DetourIATptr("MessageBoxA",(void*)NewMessageBoxA,0); //Hook the function
   MessageBoxA(NULL, "Just A MessageBox", "Just A MessageBox", 0); //Call the function -- this will invoke our fake hook.
   return 0;
}

void **IATfind(const char *function, HMODULE module) { //Find the IAT (Import Address Table) entry specific to the given function.
	int ip = 0;
	if (module == 0)
		module = GetModuleHandle(0);
	PIMAGE_DOS_HEADER pImgDosHeaders = (PIMAGE_DOS_HEADER)module;
	PIMAGE_NT_HEADERS pImgNTHeaders = (PIMAGE_NT_HEADERS)((LPBYTE)pImgDosHeaders + pImgDosHeaders->e_lfanew);
	PIMAGE_IMPORT_DESCRIPTOR pImgImportDesc = (PIMAGE_IMPORT_DESCRIPTOR)((LPBYTE)pImgDosHeaders + pImgNTHeaders->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);

	if (pImgDosHeaders->e_magic != IMAGE_DOS_SIGNATURE)
		printf("libPE Error : e_magic is no valid DOS signature\n");

	for (IMAGE_IMPORT_DESCRIPTOR *iid = pImgImportDesc; iid->Name != NULL; iid++) {
		for (int funcIdx = 0; *(funcIdx + (LPVOID*)(iid->FirstThunk + (SIZE_T)module)) != NULL; funcIdx++) {
			char *modFuncName = (char*)(*(funcIdx + (SIZE_T*)(iid->OriginalFirstThunk + (SIZE_T)module)) + (SIZE_T)module + 2);
			const uintptr_t nModFuncName = (uintptr_t)modFuncName;
			bool isString = !(nModFuncName & (sizeof(nModFuncName) == 4 ? 0x80000000 : 0x8000000000000000));
			if (isString) {
				if (!_stricmp(function, modFuncName))
					return funcIdx + (LPVOID*)(iid->FirstThunk + (SIZE_T)module);
			}
		}
	}
	return 0;
}

void DetourIATptr(const char *function, void *newfunction, HMODULE module) {
	void **funcptr = IATfind(function, module);
	if (*funcptr == newfunction)
		 return;

	DWORD oldrights, newrights = PAGE_READWRITE;
	//Update the protection to READWRITE
	VirtualProtect(funcptr, sizeof(LPVOID), newrights, &oldrights);

	RealMessageBoxA = (pMessageBoxA)*funcptr; //Some compilers require the cast (like "MinGW"), not sure about MSVC though
	*funcptr = newfunction;

	//Restore the old memory protection flags.
	VirtualProtect(funcptr, sizeof(LPVOID), oldrights, &newrights);
}
