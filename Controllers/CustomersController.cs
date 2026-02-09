using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_Final_Proj.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Adds a new customer
    /// </summary>
    /// <param name="createdCustomerRequest">The payload used to add a new customer</param>
    /// <returns>The created customer wrapped in CustomerResponseDto</returns>
    /// <response code="201">The customer was successfully added</response>
    /// <response code="400">The request body is invalid</response>
    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> AddCustomer([FromBody] CreateCustomerDto createdCustomerRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdCustomer = await _customerService.AddAsync(createdCustomerRequest);

        return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
    }

    /// <summary>
    /// Archives (soft deletes) a customer by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the customer</param>
    /// <returns>No content if archived successfully</returns>
    /// <response code="204">The customer was successfully archived</response>
    /// <response code="404">Customer not found</response>
    [HttpPatch("{id}")]
    public async Task<ActionResult<bool>> ArchiveCustomer(Guid id)
    {
        var isArchived = await _customerService.ArchiveAsync(id);

        if (!isArchived)
            return NotFound($"Customer with ID {id} NOT FOUND");

        return NoContent();
    }

    /// <summary>
    /// Permanently deletes a customer by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the customer</param>
    /// <returns>No content if deleted successfully</returns>
    /// <response code="204">The customer was successfully deleted</response>
    /// <response code="404">Customer not found</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteCustomer(Guid id)
    {
        try
        {
            var isDeleted = await _customerService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound($"Customer with ID {id} NOT FOUND");

            return NoContent();
        }
        catch
        {
            return BadRequest($"The customer you try to delete with ID {id} has got invoices. You can only delete customers to whom any invoices hasn't been sent");
        }
        
    }

    /// <summary>
    /// Updates an existing customer by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the customer</param>
    /// <param name="edittedCustomerRequest">The payload containing updated customer data</param>
    /// <returns>The updated customer wrapped in CustomerResponseDto</returns>
    /// <response code="200">The customer was successfully updated</response>
    /// <response code="400">The request body is invalid</response>
    /// <response code="404">Customer not found</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> EditCustomer(Guid id, EditCustomerDto edittedCustomerRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var edittedCustomer = await _customerService.EditAsync(id, edittedCustomerRequest);

        if (edittedCustomer is null)
            return NotFound($"Customer with ID {id} NOT FOUND");

        return Ok(edittedCustomer);
    }

    /// <summary>
    /// Retrieves all customers
    /// </summary>
    /// <returns>A list of customers</returns>
    /// <response code="200">Customers retrieved successfully</response>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAllCustomers()
    {
        var customers = await _customerService.GetAllAsync();
        if (customers is null) return BadRequest("No any customers");
        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a customer by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the customer</param>
    /// <returns>The customer wrapped in CustomerResponseDto</returns>
    /// <response code="200">Customer retrieved successfully</response>
    /// <response code="404">Customer not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetCustomerById(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);

        if (customer is null)
            return NotFound($"Customer with ID {id} NOT FOUND");

        return Ok(customer);
    }

    [HttpGet]

    public async Task<ActionResult<PagedResult<CustomerResponseDto>>> GetPaged([FromQuery] CustomerQueryParams queryParams)
    {
        var result = await _customerService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<CustomerResponseDto>>.SuccessResponse(result));
    }
}
