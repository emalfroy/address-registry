namespace AddressRegistry.StreetName
{
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Events;
    using Exceptions;

    public partial class StreetNameAddress : Entity
    {
        public StreetNameAddress AddChild(StreetNameAddress streetNameAddress)
        {
            if (_children.HasPersistentLocalId(streetNameAddress.AddressPersistentLocalId))
            {
                throw new StreetNameAddressChildAlreadyExistsException();
            }

            _children.Add(streetNameAddress);
            streetNameAddress.SetParent(this);

            return streetNameAddress;
        }

        public StreetNameAddress RemoveChild(StreetNameAddress streetNameAddress)
        {
            _children.Remove(streetNameAddress);
            streetNameAddress.SetParent(null);

            return streetNameAddress;
        }

        public bool BoxNumberIsUnique(BoxNumber boxNumber)
        {
            return _children.FirstOrDefault(x =>
                    x.IsActive
                 && x.BoxNumber is not null
                 && x.BoxNumber == boxNumber) is null;
        }

        public void Approve()
        {
            if (IsRemoved)
            {
                throw new AddressIsRemovedException(AddressPersistentLocalId);
            }

            switch (Status)
            {
                case AddressStatus.Current:
                    return;
                case AddressStatus.Retired or AddressStatus.Rejected:
                    throw new AddressCannotBeApprovedException(Status);
                case AddressStatus.Proposed:
                    Apply(new AddressWasApproved(_streetNamePersistentLocalId, AddressPersistentLocalId));
                    break;
            }
        }

        public void Reject()
        {
            if (IsRemoved)
            {
                throw new AddressIsRemovedException(AddressPersistentLocalId);
            }

            switch (Status)
            {
                case AddressStatus.Rejected:
                    return;
                case AddressStatus.Current or AddressStatus.Retired:
                    throw new AddressCannotBeRejectedException(Status);
                case AddressStatus.Proposed:
                    Apply(new AddressWasRejected(_streetNamePersistentLocalId, AddressPersistentLocalId));
                    break;
            }

            foreach (var child in _children)
            {
                child.RejectBecauseParentWasRejected();
            }
        }

        private void RejectBecauseParentWasRejected()
        {
            if (IsRemoved)
            {
                return;
            }

            if (Status == AddressStatus.Proposed)
            {
                Apply(new AddressWasRejected(_streetNamePersistentLocalId, AddressPersistentLocalId));
            }
        }

        public void Deregulate()
        {
            if (IsRemoved)
            {
                throw new AddressIsRemovedException(AddressPersistentLocalId);
            }

            var validStatuses = new[] { AddressStatus.Proposed, AddressStatus.Current };

            if (!validStatuses.Contains(Status))
            {
                throw new AddressCannotBeDeregulatedException(Status);
            }

            if (!IsOfficiallyAssigned)
            {
                return;
            }

            Apply(new AddressWasDeregulated(_streetNamePersistentLocalId, AddressPersistentLocalId));
        }

        public void Regularize()
        {
            if (IsRemoved)
            {
                throw new AddressIsRemovedException(AddressPersistentLocalId);
            }

            var validStatuses = new[] { AddressStatus.Proposed, AddressStatus.Current };

            if (!validStatuses.Contains(Status))
            {
                throw new AddressCannotBeRegularizedException(Status);
            }

            if (IsOfficiallyAssigned)
            {
                return;
            }

            Apply(new AddressWasRegularized(_streetNamePersistentLocalId, AddressPersistentLocalId));
        }

        public void Retire()
        {
            if (IsRemoved)
            {
                throw new AddressIsRemovedException(AddressPersistentLocalId);
            }

            switch (Status)
            {
                case AddressStatus.Retired:
                    return;
                case AddressStatus.Proposed or AddressStatus.Rejected:
                    throw new AddressCannotBeRetiredException(Status);
                case AddressStatus.Current:
                    Apply(new AddressWasRetiredV2(_streetNamePersistentLocalId, AddressPersistentLocalId));
                    break;
            }

            foreach (var child in _children)
            {
                child.RetireBecauseParentWasRetired();
            }
        }

        private void RetireBecauseParentWasRetired()
        {
            if (IsRemoved)
            {
                return;
            }

            if (Status == AddressStatus.Current)
            {
                Apply(new AddressWasRetiredBecauseHouseNumberWasRetired(_streetNamePersistentLocalId, AddressPersistentLocalId));
            }
        }

        /// <summary>
        /// Set the parent of the instance.
        /// </summary>
        /// <param name="parentStreetNameAddress">The parent instance.</param>
        /// <returns>The instance of which you have set the parent.</returns>
        public StreetNameAddress SetParent(StreetNameAddress? parentStreetNameAddress)
        {
            Parent = parentStreetNameAddress;
            return this;
        }
    }
}
