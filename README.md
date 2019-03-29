# Detect-single-character-XOR
基于C#，找到并解密单字符XOR加密得到的密文

## 基本思路
* 对于每一组密文，根据字母频率（计算评分的方式），猜测出最有可能的秘钥以及对应的明文，即“候选者”
* 候选者明文中不应该有乱码
* 从候选者中找到评分最高的组，认为是正确的一组

[问题来源](http://cryptopals.com/sets/1/challenges/4)
