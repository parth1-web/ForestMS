using LE.Account.Infrastructure.Dto;
using Microsoft.EntityFrameworkCore.Storage;

namespace LE.Account.Service.Services.Interface
{
    public interface TransactionService
    {
        void addTransaction(TransactionDto transactionDto);

        // Starts (or joins) an explicit database transaction on the shared scoped
        // AppDbContext — the real transaction boundary for multi-step money flows,
        // since ambient System.Transactions scopes are no-ops for EF Core here.
        // Returns null if a transaction is already open (caller joins the owner).
        IDbContextTransaction beginTransaction();
    }
}
