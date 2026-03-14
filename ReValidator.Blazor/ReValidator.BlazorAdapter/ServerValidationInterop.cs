using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace ReValidator.BlazorAdapter;

/// <summary>
/// Merges RFC 7807 validation problem details returned by the API
/// into a Blazor <see cref="ValidationMessageStore"/> so that server-side
/// errors appear alongside client-side errors on the form.
/// </summary>
public static class ServerValidationInterop
{
    /// <summary>
    /// Parses validation errors from an HTTP 400 <c>ValidationProblem</c> response body
    /// and adds them to the <paramref name="messageStore"/>.
    /// Returns <c>true</c> when the response indicates success (no errors added).
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="responseBody">The response body already read as a string.</param>
    /// <param name="editContext">The form's <see cref="EditContext"/>.</param>
    /// <param name="messageStore">The store to add server validation errors to.</param>
    public static bool HandleResponse(
        HttpResponseMessage response,
        string responseBody,
        EditContext editContext,
        ValidationMessageStore messageStore)
    {
        if (response.IsSuccessStatusCode)
            return true;

        if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors))
                {
                    foreach (var property in errors.EnumerateObject())
                    {
                        var fieldIdentifier = new FieldIdentifier(editContext.Model, property.Name);
                        var messages = property.Value.EnumerateArray()
                            .Select(e => e.GetString()!)
                            .Where(m => m is not null);
                        messageStore.Add(fieldIdentifier, messages);
                    }

                    editContext.NotifyValidationStateChanged();
                }
            }
            catch (JsonException)
            {
                // Not a valid problem details response — ignore.
            }
        }

        return false;
    }
}
