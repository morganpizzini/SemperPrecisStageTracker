using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Domain.Utils
{
    public static class ListUtils
    {
        public static void Move<T>(this IList<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > oldIndex) newIndex--;
                    // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }

        }
        public static void AddMessage(this IList<ValidationResult> validations, string message)
        {
            validations.Add(new ValidationResult(message));
        }
    }
}
