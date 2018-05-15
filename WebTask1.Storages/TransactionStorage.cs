using System.Collections.Generic;
using System.Linq;
using WebTask1.Dto;

namespace WebTask1.Storages
{
    public class TransactionStorage
    {
        private readonly List<TransactionDto> _transactionList = new List<TransactionDto>();

        public List<TransactionDto> GetAll()
        {
            if (_transactionList.Count == 0) return null;

            return _transactionList;
        }
        
        public bool AddTransaction(TransactionDto transaction)
        {
            _transactionList.Add(transaction);
            return true;
        }

        public TransactionDto GetById(string id)
        {
            return _transactionList.FirstOrDefault(transaction => transaction.GeneratedId == id);
        }

        public void InsertOrUpdate(TransactionDto newTransaction)
        {
            TransactionDto transaction = _transactionList.FirstOrDefault(trans => trans.GeneratedId == newTransaction.GeneratedId);

            if (transaction == null)
            {
                _transactionList.Add(newTransaction);
            }
            else
            {
                foreach (var trans in _transactionList)
                {
                    if (trans.GeneratedId == newTransaction.GeneratedId)
                    {
                        trans.Status = newTransaction.Status;
                    }
                }
            }
        }
    }
}
