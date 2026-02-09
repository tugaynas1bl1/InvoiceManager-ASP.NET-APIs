using ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.Models;
using AutoMapper;

namespace ASP_NET_Final_Proj.Mapping;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        // Customer
        CreateMap<Customer, CustomerResponseDto>();
        CreateMap<CreateCustomerDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        CreateMap<EditCustomerDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        // Invoice
        CreateMap<Invoice, InvoiceResponseDto>()
            .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(
                    dest => dest.TotalSum, 
                    opt => opt.MapFrom(src => src.TotalSum));

        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        CreateMap<EditInvoiceDto, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        // InvoiceRow
        CreateMap<InvoiceRow, InvoiceRowResponseDto>();
        CreateMap<CreateInvoiceRowDto, InvoiceRow>();
        CreateMap<EditInvoiceRowDto, InvoiceRow>();
    }
}
