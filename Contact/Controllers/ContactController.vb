Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http

Namespace MyMVCApp.Controllers
    Public Class ContactController
        Inherits ApiController

        Public Function GetAllContacts() As IHttpActionResult
            Dim contacts As IList(Of ContactViewModel) = Nothing

            Using ctx = New ContactEntities()
                contacts = ctx.ContactInfoes.Include("FirstName").[Select](Function(s) New ContactViewModel() With {
                    .Id = s.Id,
                    .FirstName = s.FirstName,
                    .LastName = s.LastName,
                    .Email = s.Email,
                    .PhoneNumber = s.PhoneNumber,
                    .Status = s.Status
                }).ToList()
            End Using

            If contacts.Count = 0 Then
                Return NotFound()
            End If

            Return Ok(contacts)
        End Function

        Public Function PostNewContact(ByVal contact As ContactViewModel) As IHttpActionResult
            If Not ModelState.IsValid Then Return BadRequest("Invalid data.")

            Using ctx = New ContactEntities()
                ctx.ContactInfoes.Add(New ContactInfo() With {
                    .Id = contact.Id,
                    .FirstName = contact.FirstName,
                    .LastName = contact.LastName,
                    .Email = contact.Email,
                    .PhoneNumber = contact.PhoneNumber,
                    .Status = contact.Status
                })
                ctx.SaveChanges()
            End Using

            Return Ok()
        End Function

        Public Function Put(ByVal contact As ContactViewModel) As IHttpActionResult
            If Not ModelState.IsValid Then Return BadRequest("Not a valid model")

            Using ctx = New ContactEntities()
                Dim existingContact = ctx.ContactInfoes.Where(Function(s) s.Id = contact.Id).FirstOrDefault()

                If existingContact IsNot Nothing Then
                    existingContact.FirstName = contact.FirstName
                    existingContact.LastName = contact.LastName
                    existingContact.Email = contact.Email
                    existingContact.PhoneNumber = contact.PhoneNumber
                    existingContact.Status = contact.Status
                    ctx.SaveChanges()
                Else
                    Return NotFound()
                End If
            End Using

            Return Ok()
        End Function

        Public Function Delete(ByVal id As Integer) As IHttpActionResult
            If id <= 0 Then Return BadRequest("Not a valid contact id")

            Using ctx = New ContactEntities()
                Dim contact = ctx.ContactInfoes.Where(Function(s) s.Id = id).FirstOrDefault()
                ctx.Entry(contact).State = System.Data.Entity.EntityState.Deleted
                ctx.SaveChanges()
            End Using

            Return Ok()
        End Function
    End Class
End Namespace