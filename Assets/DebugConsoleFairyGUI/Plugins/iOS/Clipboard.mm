#include "Clipboard.h"
@implementation Clipboard
//将文本复制到IOS剪贴板
- (void)objc_copy : (NSString*)text
{
     UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
     pasteboard.string = text;
}
@end

extern "C" {
     static Clipboard *iosClipboard;
   
     void _copy(const char *textList)
    {   
        NSString *text = [NSString stringWithUTF8String: textList] ;
       
        if(iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }
       
        [iosClipboard objc_copy: text];
    }

}
