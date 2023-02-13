namespace Antiban
{
    public class Antiban //Prev Class Name Antiban
    {
        private List<EventDTO> _incomedDTO = new();
        /// <summary>
        /// Добавление сообщений в систему, для обработки порядка сообщений
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PushEventMessage(EventMessage eventMessage)
        {

            _incomedDTO.Add(new(eventMessage));

        }
        /// <summary>
        /// Вовзращает порядок отправок сообщений
        /// </summary>
        /// <returns></returns>
        public List<AntibanResult> GetResult()
        {
            SortByFirstRule();

            SortMessagePerPhoneNumber();

            SortMessageForMailing();

            _incomedDTO.Sort((x, y) => DateTime.Compare(x.EstimatedSentTime, y.EstimatedSentTime));

            var messagesOrder = new List<AntibanResult>();
            foreach (var item in _incomedDTO)
            {
                messagesOrder.Add(item.ConvertToAntibanResult());
            }

            return messagesOrder;
        }
        private void SortByFirstRule()
        {
            _incomedDTO.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));

            DateTime compareTime = new();
            for (int i = 0; i < _incomedDTO.Count; i++)
            {
                if (_incomedDTO[i].IsSortedByMainRule)
                    continue;

                if (i == 0)
                {
                    compareTime = _incomedDTO[i].EstimatedSentTime;
                    continue;
                }

                if ((_incomedDTO[i].EstimatedSentTime - compareTime).TotalSeconds < SortRules.IntervalPerMessageGlobal.TotalSeconds)
                {
                    _incomedDTO[i].EstimatedSentTime = compareTime.AddSeconds(SortRules.IntervalPerMessageGlobal.TotalSeconds);
                }
                compareTime = _incomedDTO[i].EstimatedSentTime;
            }
        }
        private void SortMessagePerPhoneNumber()
        {
            foreach (var message in _incomedDTO)
            {
                var messagePerNumber = _incomedDTO.Where(m => m.Phone == message.Phone && m.MessageId < message.MessageId && m.EstimatedSentTime <= message.EstimatedSentTime).LastOrDefault();

                if (messagePerNumber == null)
                    continue;

                if ((message.EstimatedSentTime - messagePerNumber.EstimatedSentTime).TotalSeconds < SortRules.IntervalToNumber.TotalSeconds)
                {
                    message.EstimatedSentTime = messagePerNumber.EstimatedSentTime.AddMinutes(SortRules.IntervalToNumber.TotalMinutes);
                    message.IsSortedByMainRule = true;
                }
            }
        }
        private void SortMessageForMailing()
        {
            foreach (var message in _incomedDTO)
            {
                if (message.Priority == 0)
                    continue;

                var mailing = _incomedDTO.Where(m => m.Priority == 1 && m.Phone == message.Phone && m.MessageId < message.MessageId).LastOrDefault();

                if (mailing == null)
                    continue;

                if ((message.EstimatedSentTime - mailing.EstimatedSentTime).TotalSeconds < SortRules.IntervalToMailing.TotalSeconds)
                {
                    message.EstimatedSentTime = mailing.EstimatedSentTime.AddHours(SortRules.IntervalToMailing.TotalHours);
                    message.IsSortedByMainRule = true;
                }
            }
        }
    }
}
