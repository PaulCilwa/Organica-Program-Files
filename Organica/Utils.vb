Imports System.Xml

Module Utils

    Public ReadOnly NothingDate As Date

    Public Function HTML_Encode(ByVal Value As String) As String
        Value = Value.Replace("&", "&amp;")
        Value = Value.Replace("--", "&mdash;")
        Value = Value.Replace("—", "&mdash;")
        Value = Value.Replace(Chr(150), "&mdash;")
        Value = Value.Replace("é", "&eacute;")
        Value = Value.Replace("""", "&quot;")
        Value = Value.Replace(ChrW(8220), "&quot;")
        Value = Value.Replace(ChrW(8221), "&quot;")
        Value = Value.Replace("'", "&apos;")
        Value = Value.Replace("…", "&hellip;")
        Value = Value.Replace("...", "&hellip;")
        Value = Value.Replace("½", "&frac12;")
        Value = Value.Replace("¼", "&frac14;")
        Value = Value.Replace("¾", "&frac34;")
        Value = Value.Replace("ï", "&iuml;")
        Value = Value.Replace("™", "&trade;")
        Value = Value.Replace("\", "&#92;")
        Return Value
    End Function

    Public Function HTML_Decode(ByVal Value As String) As String
        Value = Value.Replace("&amp;", "&")
        Value = Value.Replace("&mdash;", "—")
        Value = Value.Replace("&eacute;", "é")
        Value = Value.Replace("&quot;", """")
        Value = Value.Replace("&apos;", "'")
        Value = Value.Replace("&hellip;", "…")
        Value = Value.Replace("&frac12;", "½")
        Value = Value.Replace("&frac14;", "¼")
        Value = Value.Replace("&frac34;", "¾")
        Value = Value.Replace("&iuml;", "ï")
        Value = Value.Replace("&trade;", "™")
        Value = Value.Replace("&#92;", "\")
        Return Value
    End Function

    Function ToBool(ByVal Value) As Boolean
        If Value Is Nothing Then
            Return False
        End If

        If VarType(Value) = vbBoolean Then
            Return Value
        End If

        If VarType(Value) = vbInteger Or VarType(Value) = vbLong Then
            Return IIf(Value = 0, False, True)
        End If

        Select Case UCase(Value)
            Case "TRUE", "YES", "ON", "1", "T", "Y"
                ToBool = True
            Case Else
                ToBool = False
        End Select
    End Function

    Public Function SafeNode(ByRef Node As XmlNode, ByVal FieldName As String) As String
        Dim Result As Object = Node.SelectSingleNode(FieldName)
        If Result Is Nothing Then
            Return String.Empty
        Else
            Return Result.InnerText
        End If
    End Function

End Module
