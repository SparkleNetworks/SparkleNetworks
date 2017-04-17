//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sparkle.Entities.Networks.Neutral
{
    public partial class PollAnswerPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int PollId
        {
            get { return _pollId; }
            set
            {
                if (this._pollId != value)
                {
                    if (this.Poll != null && this.Poll.Id != value)
                    {
                        this.Poll = null;
                    }
                    this._pollId = value;
                }
            }
        }
        private int _pollId;
    
        public int ChoiceId
        {
            get { return _choiceId; }
            set
            {
                if (this._choiceId != value)
                {
                    if (this.PollChoice != null && this.PollChoice.Id != value)
                    {
                        this.PollChoice = null;
                    }
                    this._choiceId = value;
                }
            }
        }
        private int _choiceId;
    
        public int UserId
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual PollChoicePoco PollChoice
        {
            get { return _pollChoice; }
            set
            {
                if (!ReferenceEquals(_pollChoice, value))
                {
                    var previousValue = _pollChoice;
                    _pollChoice = value;
                    FixupPollChoice(previousValue);
                }
            }
        }
        private PollChoicePoco _pollChoice;
    
        public virtual PollPoco Poll
        {
            get { return _poll; }
            set
            {
                if (!ReferenceEquals(_poll, value))
                {
                    var previousValue = _poll;
                    _poll = value;
                    FixupPoll(previousValue);
                }
            }
        }
        private PollPoco _poll;

        #endregion

        #region Association Fixup
    
        private void FixupPollChoice(PollChoicePoco previousValue)
        {
            if (previousValue != null && previousValue.PollAnswers.Contains(this))
            {
                previousValue.PollAnswers.Remove(this);
            }
    
            if (PollChoice != null)
            {
                if (!PollChoice.PollAnswers.Contains(this))
                {
                    PollChoice.PollAnswers.Add(this);
                }
                if (ChoiceId != PollChoice.Id)
                {
                    ChoiceId = PollChoice.Id;
                }
            }
        }
    
        private void FixupPoll(PollPoco previousValue)
        {
            if (previousValue != null && previousValue.PollAnswers.Contains(this))
            {
                previousValue.PollAnswers.Remove(this);
            }
    
            if (Poll != null)
            {
                if (!Poll.PollAnswers.Contains(this))
                {
                    Poll.PollAnswers.Add(this);
                }
                if (PollId != Poll.Id)
                {
                    PollId = Poll.Id;
                }
            }
        }

        #endregion

    }
}
