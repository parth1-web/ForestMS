using AutoMapper;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Web.Areas.Accounting.Models;

namespace LE.Web.Areas.Accounting.AutomapperProfiles
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<LedgerGroup, LedgerGroupModel>().ReverseMap();
            CreateMap<Ledger, LedgerModel>().ReverseMap();
            CreateMap<LedgerModel, LedgerDto>().ReverseMap();
            CreateMap<PaymentModel, PaymentDto>().ReverseMap();
            CreateMap<ReceiptModel, ReceiptDto>().ReverseMap();
            CreateMap<JournalModel, JournalDetailDto>().ReverseMap();
            CreateMap<ReceiptModel, Receipt>().ReverseMap();
            CreateMap<JournalModelDetails, JournalDetailDto>().ReverseMap();
            // CreateMap<Transaction, TransactionModel>();
        }
    }
}
