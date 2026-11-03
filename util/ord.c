#include <stdio.h>
#include <string.h>

int main(int argc, char** argv){
    printf("argc: %d\n", argc);
    printf("argv[0]: %s\n", argv[0]);
    if(argc>1){
        size_t l = strlen(argv[1]);
        printf("argv[1]: %s\n", argv[1]);
        printf("strlen(argv[1]): %d\n", l);
        size_t i;
        for(i=0;i<l;i++){
            printf("%d ", argv[1][i]);
        }
        puts("");
    }
    return 0;
}
