using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Services.Interface
{
    public interface JournalService
    {
        void makeJournalEntries(JournalDto journalDto);
    }
}
