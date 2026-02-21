Imports System.Text.RegularExpressions

Module CommonLicence

    Public strPubKey As String = "<RSAKeyValue><Modulus>q53+M7Brxnifa3hw7Bn5PuEiX+QpYPE33OAWDKmy3vPllmkRuxyfjquOeLVbHkTSaoA1qPYirxfayRwRUoCwvWGSqz84qQpE4fhU1STem201M5RnJx3fxXiUduL4Nxs3tk5XrlDVY1lQWXKDomGBiKS+3T6B+ljH4HpQmF973yU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
    Public strPriKey As String = "<RSAKeyValue><Modulus>q53+M7Brxnifa3hw7Bn5PuEiX+QpYPE33OAWDKmy3vPllmkRuxyfjquOeLVbHkTSaoA1qPYirxfayRwRUoCwvWGSqz84qQpE4fhU1STem201M5RnJx3fxXiUduL4Nxs3tk5XrlDVY1lQWXKDomGBiKS+3T6B+ljH4HpQmF973yU=</Modulus><Exponent>AQAB</Exponent><P>2AB9nfkOs/3xIIJAZbfBAfNvP09mucBHC2HE/crCmEzxPHCjjLhfdg2zORmg1Ua4t0zIpVBFMv3yx8EJjgRZBQ==</P><Q>y2V0nmLRQB55ns9hi2jKN/C/QrZWWQdCm7yrgv30Z9PBGAQve3frWt+p81byKUQ0yNu02jKXKAPerf39iOnHoQ==</Q><DP>h9EHK2WHETYDf+VmiI7aFVf0A2LxvKpiAY4gR1ROt2Tp6o8Ix1rG63wBzU2IC5LEYr0tDIVEfaOgHGoMj/e74Q==</DP><DQ>fqCWPvkkbvfKHe3cO6+snbEbUcw0685SUKTgXnf+jhlOEMaiTQr2kqfGpcGOl9RnzFjEOkfexUHLg6UqD/ADoQ==</DQ><InverseQ>wA9PtlSGjzJqV5JmGnrpMN3B0BEXxvxS15tQr1HB8I86KN1klLesJoB90vpuawoAR+5BUHHFz3RGXghpnHuDNg==</InverseQ><D>Py4ACol1c/CuSANkFxeM0eBSJlk5/o1vUmpQ08KZrki+CfyOYYMtHnn8DmY9sEwH5ttiZdyPckRm8Ejb+7KS3dsqXcY2//SfWxzrF6uB2jb6KdLVeBOzjToUqE73S8MtWvJg2PfKh9iWEGe/1pdJUF5M2dzX0PO/OYImEQVT4YE=</D></RSAKeyValue>"

    Public Sub CreateKeys(ByRef publicKey As String, _
                                 ByRef privateKey As String)
        'RSACryptoServiceProviderオブジェクトの作成
        Dim rsa As New System.Security.Cryptography.RSACryptoServiceProvider()

        '公開鍵をXML形式で取得
        publicKey = rsa.ToXmlString(False)
        '秘密鍵をXML形式で取得
        privateKey = rsa.ToXmlString(True)
    End Sub

    Public Function Encrypt(ByVal intKey As Integer) As String
        Dim str As String
        Dim intVal As Integer

        '        intVal = (intVal * 3.456789) / 6.54321 + ((intVal * 7.89723498) + (771438641233 / (intVal * 3.456789))) Mod 100000000
        intVal = (intKey + ((intKey Mod 2 * 1231.234) + (intKey Mod 3 * 23453.345) + (intKey Mod 4 * 335345345.456) + (intKey Mod 4 * 34545.456))) Mod 100000000
        str = CStr(intVal)
        str = New String(" ", 8 - str.Length) & str

        Return str
        '        Return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str))

    End Function

    ''' <summary>
    ''' 公開鍵を使って文字列を暗号化する
    ''' </summary>
    ''' <param name="str">暗号化する文字列</param>
    ''' <param name="publicKey">暗号化に使用する公開鍵(XML形式)</param>
    ''' <returns>暗号化された文字列</returns>
    Public Function Encrypt(ByVal str As String, _
                                   ByVal publicKey As String) As String
        Return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str))


        'RSACryptoServiceProviderオブジェクトの作成
        Dim rsa As New System.Security.Cryptography.RSACryptoServiceProvider()

        '公開鍵を指定
        rsa.FromXmlString(publicKey)

        '暗号化する文字列をバイト配列に
        Dim data As Byte() = System.Text.Encoding.UTF8.GetBytes(str)
        '暗号化する
        '（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
        Dim encryptedData As Byte() = rsa.Encrypt(data, False)

        'Base64で結果を文字列に変換
        Return System.Convert.ToBase64String(encryptedData)
    End Function

    ''' <summary>
    ''' 秘密鍵を使って文字列を復号化する
    ''' </summary>
    ''' <param name="str">Encryptメソッドにより暗号化された文字列</param>
    ''' <param name="privateKey">復号化に必要な秘密鍵(XML形式)</param>
    ''' <returns>復号化された文字列</returns>
    Public Function Decrypt(ByVal str As String, _
                                   ByVal privateKey As String) As String
        'RSACryptoServiceProviderオブジェクトの作成
        Dim rsa As New System.Security.Cryptography.RSACryptoServiceProvider()

        '秘密鍵を指定
        rsa.FromXmlString(privateKey)

        '復号化する文字列をバイト配列に
        Dim data As Byte() = System.Convert.FromBase64String(Str)
        '復号化する
        Dim decryptedData As Byte() = rsa.Decrypt(data, False)

        '結果を文字列に変換
        Return System.Text.Encoding.UTF8.GetString(decryptedData)
    End Function

    Public Function Get8Moji(ByVal strKey As String) As String
        Get8Moji = Mid(Regex.Replace(strKey, "[^A-Z]", "", RegexOptions.IgnoreCase), 1, 8)
    End Function

    Public Function ExchageKey(ByVal strKey As String) As String
        Dim intCnt As Integer
        Dim strNo As String = Mid(strKey, 1, 8)
        Dim strEnc As String = Mid(strKey, 9)

        ExchageKey = ""
        For intCnt = 1 To 8
            ExchageKey &= Mid(strNo, intCnt, 1)
            ExchageKey &= Mid(strEnc, 9 - intCnt, 1)
        Next
    End Function

    Public Function DeExchageKey(ByVal strKey As String) As String
        Dim intCnt As Integer
        Dim strNo As String = ""
        Dim strEnc As String = ""

        DeExchageKey = ""
        For intCnt = 1 To 16 Step 2
            strNo &= Mid(strKey, intCnt, 1)
        Next
        For intCnt = 16 To 1 Step -2
            strEnc &= Mid(strKey, intCnt, 1)
        Next
        DeExchageKey = strNo & strEnc
    End Function

End Module
