Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Pdf
Imports System.IO
Imports DevExpress.Web.ASPxEditors
Imports DevExpress.Web.ASPxDataView
Imports System.Drawing
Imports System.Drawing.Imaging

Namespace E5095

    Public Partial Class PdfViewer
        Inherits System.Web.UI.UserControl

        Private _pdfData As Byte()

        Private _pdfFilePath As String

        Private _documentProcessor As DevExpress.Pdf.PdfDocumentProcessor

        Public Sub New()
            Me._pdfData = Nothing
            Me._pdfFilePath = ""
            Me._documentProcessor = New DevExpress.Pdf.PdfDocumentProcessor()
        End Sub

        Protected ReadOnly Property DocumentProcessor As PdfDocumentProcessor
            Get
                Return Me._documentProcessor
            End Get
        End Property

        Public Property Width As Unit
            Get
                Return Me.dvDocument.Width
            End Get

            Set(ByVal value As Unit)
                Me.dvDocument.Width = value
            End Set
        End Property

        Public Property Height As Unit
            Get
                Return Me.dvDocument.Height
            End Get

            Set(ByVal value As Unit)
                Me.dvDocument.Height = value
            End Set
        End Property

        Public Property PdfFilePath As String
            Get
                Return Me._pdfFilePath
            End Get

            Set(ByVal value As String)
                Me._pdfFilePath = value
                If Not System.[String].IsNullOrEmpty(value) Then
                    Me.DocumentProcessor.LoadDocument(Me.Server.MapPath(value))
                    Me.BindDataView()
                End If
            End Set
        End Property

        Public Property PdfData As Byte()
            Get
                Return Me._pdfData
            End Get

            Set(ByVal value As Byte())
                Me._pdfData = value
                If value IsNot Nothing Then
                    Using stream As System.IO.MemoryStream = New System.IO.MemoryStream(value)
                        Me.DocumentProcessor.LoadDocument(stream)
                        Me.BindDataView()
                    End Using
                End If
            End Set
        End Property

        Protected Sub BindDataView()
            If Me.DocumentProcessor.Document IsNot Nothing Then
                Dim data As System.Collections.Generic.List(Of E5095.PdfViewer.PdfPageItem) = New System.Collections.Generic.List(Of E5095.PdfViewer.PdfPageItem)()
                For pageNumber As Integer = 1 To Me.DocumentProcessor.Document.Pages.Count
                    data.Add(New E5095.PdfViewer.PdfPageItem() With {.PageNumber = pageNumber})
                Next

                Me.dvDocument.DataSource = data
                Me.dvDocument.DataBind()
            End If
        End Sub

        Protected Sub bimPdfPage_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim image As DevExpress.Web.ASPxEditors.ASPxBinaryImage = TryCast(sender, DevExpress.Web.ASPxEditors.ASPxBinaryImage)
            Dim container As DevExpress.Web.ASPxDataView.DataViewItemTemplateContainer = TryCast(image.NamingContainer, DevExpress.Web.ASPxDataView.DataViewItemTemplateContainer)
            Dim pageNumber As Integer = CInt(container.EvalDataItem("PageNumber"))
            Using bitmap As System.Drawing.Bitmap = Me.DocumentProcessor.CreateBitmap(pageNumber, 900)
                Using stream As System.IO.MemoryStream = New System.IO.MemoryStream()
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png)
                    image.ContentBytes = stream.ToArray()
                End Using
            End Using
        End Sub

        Protected Class PdfPageItem

            Public Property PageNumber As Integer
        End Class
    End Class
End Namespace