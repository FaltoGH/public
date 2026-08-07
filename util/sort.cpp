#include <stdio.h>
#include <iostream>
#include <vector>
#include <algorithm>

int main(int argc,char**argv){
  std::vector<std::string> vecto;
  std::string strin;
  
  while(1){
    std::getline(std::cin, strin);
    if(strin==""){
      break;
    }
    vecto.push_back(strin);
  }
  
  std::sort(vecto.begin(), vecto.end());
  for(auto s : vecto){
    std::cout << s << "\r\n";
  }
  
  return 0;
}

