@ interface Clipboard : NSObject

extern "C"
{
    /*  compare the namelist with system processes  */
    void _copy(const char *textList);
}

@end