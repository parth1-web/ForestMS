using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Linq;
using System.Transactions;

namespace LE.Account.Service.Services.Implementations
{
    public class LedgerBalanceServiceImpl : LedgerBalanceService
    {
        private readonly LedgerBalanceRepository _ledgerBalanceRepo;
        private readonly LedgerBalanceAssembler _ledgerBalanceMaker;

        public LedgerBalanceServiceImpl(LedgerBalanceRepository ledgerBalanceRepo, LedgerBalanceAssembler ledgerBalanceMaker)
        {
            _ledgerBalanceRepo = ledgerBalanceRepo;
            _ledgerBalanceMaker = ledgerBalanceMaker;
        }

        public void update(LedgerBalanceDto ledgerBalanceDto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var ledgerBalance = _ledgerBalanceRepo.getQueryable().Where(a => a.ledger_id == ledgerBalanceDto.ledger_id).FirstOrDefault();
                    if (ledgerBalance == null)
                        throw new ItemNotFoundException($"Ledger with id {ledgerBalanceDto.ledger_id} doesnot exist.");

                    ledgerBalanceDto.ledger_balance_id = ledgerBalance.ledger_balance_id;
                    _ledgerBalanceMaker.copy(ledgerBalance, ledgerBalanceDto);
                    _ledgerBalanceRepo.update(ledgerBalance);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void save(LedgerBalanceDto ledgerBalanceDto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var ledgerBalance = new Entities.LedgerBalance();

                        _ledgerBalanceMaker.copy(ledgerBalance, ledgerBalanceDto);
                        _ledgerBalanceRepo.insert(ledgerBalance);
                        tx.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (TransactionAbortedException ex)
            {
                throw ex;
            }
        }
    }
}
