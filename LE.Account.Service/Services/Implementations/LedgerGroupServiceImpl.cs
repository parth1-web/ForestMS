using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Linq;

namespace LE.Account.Service.Services.Implementations
{
    public class LedgerGroupServiceImpl : LedgerGroupService
    {
        private readonly LedgerGroupRepository _ledgerGroupRepo;

        public LedgerGroupServiceImpl(LedgerGroupRepository ledgerGroupRepo)
        {
            _ledgerGroupRepo = ledgerGroupRepo;
        }

        public void delete(long ledger_group_id)
        {
            try
            {
                using (var tx = _ledgerGroupRepo.beginTransaction())
                {
                    var ledgerGroup = _ledgerGroupRepo.getById(ledger_group_id);
                    if (ledgerGroup == null)
                        throw new ItemNotFoundException($"Ledger group with id{ledger_group_id} doesn't exist.");

                    if (ledgerGroup.getLedgersCount() > 0)
                        throw new ItemUsedException("Specified Ledger group has already been assigned to some menu items.");

                    if (ledgerGroup.is_custom == false)
                    {
                        throw new Exception("You are not allowed to delete primary Group.");
                    }
                    _ledgerGroupRepo.delete(ledgerGroup);
                    _ledgerGroupRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void save(LedgerGroup ledger_group)
        {
            try
            {
                using (var tx = _ledgerGroupRepo.beginTransaction())
                {

                    var ledgerGroupWithSameName = _ledgerGroupRepo.getByName(ledger_group.name);
                    bool isNameAllowed = ledgerGroupWithSameName == null;

                    if (!isNameAllowed)
                        throw new DuplicateItemException($"Ledger group with {ledger_group.name} already exist.");

                    if (ledger_group.parent_ledger_group_id > 0)
                    {
                        ledger_group.ledger_group_type = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_id == ledger_group.parent_ledger_group_id).FirstOrDefault().ledger_group_type;
                    }
                    string newCode = getLedgerGroupCode(ledger_group);

                    //  var ledgerGroupTypeNumber = ledger_group.ledger_group_type;
                    ledger_group.code = newCode;

                    _ledgerGroupRepo.insert(ledger_group);
                    _ledgerGroupRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string getLedgerGroupCode(LedgerGroup ledger_group)
        {
            var ledgerGroupList = _ledgerGroupRepo.getLedgerGroupByAccountType(ledger_group.ledger_group_type);
            var newCode = string.Concat((int)ledger_group.ledger_group_type, ".1");

            if (ledgerGroupList.Count != 0)
            {
                var lastLedgerGroupIdonParticularType = ledgerGroupList[ledgerGroupList.Count - 1].code;
                string[] parts = lastLedgerGroupIdonParticularType.Split('.');
                var front = int.Parse(parts[0]);
                int last = int.Parse(parts[1]) + 1;
                newCode = string.Concat(front, ".", last);
            }

            return newCode;
        }

        public void update(LedgerGroup ledger_group)
        {
            try
            {
                using (var tx = _ledgerGroupRepo.beginTransaction())
                {

                    var ledgerGroup = _ledgerGroupRepo.getById(ledger_group.ledger_group_id);
                    if (ledgerGroup == null)
                        throw new ItemNotFoundException($"Ledger Group with id {ledgerGroup.ledger_group_id} doesnot exist.");

                    var ledgerGroupWithSameName = _ledgerGroupRepo.getByName(ledgerGroup.name);
                    bool isNameAllowed = (ledgerGroupWithSameName == null || ledgerGroupWithSameName.ledger_group_id == ledger_group.ledger_group_id);

                    if (!isNameAllowed)
                        throw new DuplicateItemException($"Ledger group {ledger_group.name} already exists.");


                    ledgerGroup.name = ledger_group.name;
                    ledgerGroup.code = ledger_group.code;
                    ledgerGroup.parent_ledger_group_id = ledger_group.parent_ledger_group_id;
                    //  ledgerGroup.ledger_group_type = ledger_group.ledger_group_type;




                    if (ledger_group.parent_ledger_group_id > 0)
                    {
                        ledger_group.ledger_group_type = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_id == ledger_group.parent_ledger_group_id).FirstOrDefault().ledger_group_type;
                    }

                    if (ledgerGroup.ledger_group_type != ledger_group.ledger_group_type)
                    {
                        ledgerGroup.code = getLedgerGroupCode(ledger_group);
                    }
                    ledgerGroup.ledger_group_type = ledger_group.ledger_group_type;
                    _ledgerGroupRepo.update(ledgerGroup);
                    _ledgerGroupRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
