# -*- coding: utf-8 -*-
import re
# 유니코드 한글 시작 : 44032, 끝 : 55199
BASE_CODE, CHOSUNG, JUNGSUNG = 44032, 588, 28

# 초성 리스트. 00 ~ 18
CHOSUNG_LIST = ['ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ']

# 중성 리스트. 00 ~ 20
JUNGSUNG_LIST = ['ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ']

# 종성 리스트. 00 ~ 27 + 1(1개 없음)
JONGSUNG_LIST = [' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ']

tc = ['Blog민트', '육손', '밥먹어친구야', '타키매너59']

c2a = {
'ㄱ': 'r', 
'ㄲ': 'R', 
'ㄴ': 's', 
'ㄷ': 'e', 
'ㄸ': 'E', 
'ㄹ': 'f', 
'ㅁ': 'a', 
'ㅂ': 'q', 
'ㅃ': 'Q', 
'ㅅ': 't', 
'ㅆ': 'T', 
'ㅇ': 'd', 
'ㅈ': 'w', 
'ㅉ': 'W', 
'ㅊ': 'c', 
'ㅋ': 'z', 
'ㅌ': 'x', 
'ㅍ': 'v', 
'ㅎ': 'g'
}

ju2a = {
'ㅏ': 'k', 
'ㅐ': 'o', 
'ㅑ': 'i', 
'ㅒ': 'O', 
'ㅓ': 'j', 
'ㅔ': 'p', 
'ㅕ': 'u', 
'ㅖ': 'P', 
'ㅗ': 'h', 
'ㅘ': 'hk', 
'ㅙ': 'ho', 
'ㅚ': 'hl', 
'ㅛ': 'y', 
'ㅜ': 'n', 
'ㅝ': 'nj', 
'ㅞ': 'np', 
'ㅟ': 'nl', 
'ㅠ': 'b', 
'ㅡ': 'm', 
'ㅢ': 'ml', 
'ㅣ': 'l'
}

jo2a = {
' ':  '',
'ㄱ': 'r', 
'ㄲ': 'R', 
'ㄳ': 'rt', 
'ㄴ': 's', 
'ㄵ': 'sw', 
'ㄶ': 'sg', 
'ㄷ': 'e', 
'ㄹ': 'f', 
'ㄺ': 'fr', 
'ㄻ': 'fa', 
'ㄼ': 'fq', 
'ㄽ': 'ft', 
'ㄾ': 'fx', 
'ㄿ': 'fv', 
'ㅀ': 'fg', 
'ㅁ': 'a', 
'ㅂ': 'q', 
'ㅄ': 'qt', 
'ㅅ': 't', 
'ㅆ': 'T', 
'ㅇ': 'd', 
'ㅈ': 'w', 
'ㅊ': 'c', 
'ㅋ': 'z', 
'ㅌ': 'c', 
'ㅍ': 'v', 
'ㅎ': 'g'
}

if __name__ == '__main__':
    test_keyword = 'ids'
    split_keyword_list = list(test_keyword)
    print(split_keyword_list)

    result = list()
    out = list()
    for keyword in split_keyword_list:
        # 한글 여부 check 후 분리
        if re.match('.*[ㄱ-ㅎㅏ-ㅣ가-힣]+.*', keyword) is not None:
            char_code = ord(keyword) - BASE_CODE
            char1 = int(char_code / CHOSUNG)
            result.append(CHOSUNG_LIST[char1])
            out.append(c2a[CHOSUNG_LIST[char1]])
            char2 = int((char_code - (CHOSUNG * char1)) / JUNGSUNG)
            out.append(ju2a[JUNGSUNG_LIST[char2]])
            result.append(JUNGSUNG_LIST[char2])
            char3 = int((char_code - (CHOSUNG * char1) - (JUNGSUNG * char2)))
            out.append(jo2a[JONGSUNG_LIST[char3]])
            result.append(JONGSUNG_LIST[char3])
        else:
            out.append(keyword)
            result.append(keyword)
    # result
    print("".join(result))
    print("".join(out))
    
