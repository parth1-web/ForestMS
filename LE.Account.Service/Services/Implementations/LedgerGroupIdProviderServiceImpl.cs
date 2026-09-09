using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Account.Service.Services.Implementations
{
    public class LedgerGroupIdProviderServiceImpl : LedgerGroupIdProviderService
    {
        private readonly LedgerGroupRepository _ledgerGroupRepo;
        private readonly LedgerSetupRepository _ledgerSetupRepository;

        public LedgerGroupIdProviderServiceImpl(LedgerGroupRepository ledgerGroupRepo, LedgerSetupRepository ledgerSetupRepository)
        {
            _ledgerGroupRepo = ledgerGroupRepo;
            _ledgerSetupRepository = ledgerSetupRepository;
        }

        public long getCreditorsGroupId()
        {
            var debtorsGroup = _ledgerSetupRepository.getByKey(LedgerSetup.Creditors_Group.ToString()) ?? throw new ItemNotFoundException("Creditors group is not set.");
            return Convert.ToInt32(debtorsGroup.value);
        }

        public long getDebtorsGroupId()
        {
            var debtorsGroup = _ledgerSetupRepository.getByKey(LedgerSetup.Debtors_Group.ToString()) ?? throw new ItemNotFoundException("Debtors group is not set.");
            return Convert.ToInt32(debtorsGroup.value);
        }
    }
}
