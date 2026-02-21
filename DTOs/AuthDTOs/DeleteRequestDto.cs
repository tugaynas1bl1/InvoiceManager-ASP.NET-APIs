namespace ASP_NET_Final_Proj.DTOs.AuthDTOs;

public class DeleteRequestDto
{
    /// <summary>
    /// Write "yes" or "no" to approve or disapprove deletion of account
    /// </summary>
    /// <example>no</example>
    public string DeleteOrNot {  get; set; }
}
