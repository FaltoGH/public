#include <iostream>
#include <windows.h>
 
using namespace std;
 
class VirtualClass
{
public:
 
    int number;
 
    virtual void VirtualFn1() //This is the virtual function that will be hooked.
    {
        cout << "VirtualFn1 called " << number++ << "\n";
    }
};


using VirtualFn1_t = void(__thiscall*)(void* thisptr); 
VirtualFn1_t orig_VirtualFn1;

void __fastcall hkVirtualFn1(void* thisptr, int edx) //This is our hook function which we will cause the program to call instead of the original VirtualFn1 function after hooking is done.
{
    cout << "\nHook function called\n";
    orig_VirtualFn1(thisptr); //Call the original function.
    cout << "Hook function end\n\n";
}


int main()
{
    VirtualClass* myClass = new VirtualClass(); //Create a pointer to a dynamically allocated instance of VirtualClass.
    myClass->VirtualFn1();
    myClass->VirtualFn1();
    myClass->VirtualFn1();
    
    void** vTablePtr = *reinterpret_cast<void***>(myClass); //Find the address that points to the base of VirtualClass' VMT (which then points to VirtualFn1) and store it in vTablePtr.
 
    DWORD oldProtection;
    VirtualProtect(vTablePtr, 4, PAGE_EXECUTE_READWRITE, &oldProtection); //Removes page protection at the start of the VMT so we can overwrite its first pointer.
 
    orig_VirtualFn1 = reinterpret_cast<VirtualFn1_t>(*vTablePtr); //Stores the pointer to VirtualFn1 from the VMT in a global variable so that it can be accessed again later after its entry in the VMT has been 
                                                                  //overwritten with our hook function.
 
    *vTablePtr = (void*)hkVirtualFn1; //Overwrite the pointer to VirtualFn1 within the virtual table to a pointer to our hook function (hkVirtualFn1).
 
    VirtualProtect(vTablePtr, 4, oldProtection, 0); //Restore old page protection.
 
    myClass->VirtualFn1(); //Call the virtual function from our class instance. Because it is now hooked, this will actually call our hook function (hkVirtualFn1).
    myClass->VirtualFn1();
    myClass->VirtualFn1();
 
    delete myClass;
 
    return 0;
}

