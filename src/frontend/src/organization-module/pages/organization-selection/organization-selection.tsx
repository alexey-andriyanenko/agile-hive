import React from "react";
import { observer } from "mobx-react-lite";
import { Box } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { useModalsStore, useOrganizationStore } from "../../store";
import { OrganizationCard } from "./organization-card";
import type { OrganizationModel } from "../../models/organization.ts";
import { AddOrganizationCard } from "./add-organization-card";

const OrganizationSelection: React.FC = observer(() => {
  const navigate = useNavigate();
  const organizationStore = useOrganizationStore();
  const modalsStore = useModalsStore();
  const [loading, setLoading] = React.useState(false);

  React.useEffect(() => {
    if (organizationStore.organizations.length > 0) {
      return;
    }

    setLoading(true);
    organizationStore
      .fetchOrganizations()
      .then(() => setLoading(false))
      .catch((error) => {
        console.error("Failed to fetch organizations:", error);
        setLoading(false);
      });
  }, [organizationStore]);

  const handleSelect = (organization: OrganizationModel) => {
    organizationStore.setCurrentOrganization(organization);
    navigate(`/organization/${organization.slug}`);
  };

  const handleCreate = () => {
    modalsStore.open("CreateOrEditOrganizationDialog", {
      onCreate: (values) =>
        organizationStore.createOrganization({
          organizationName: values.name,
        }),
    });
  };

  return (
    <Box
      flex="1"
      display="grid"
      gridTemplateColumns="1fr 1fr 1fr"
      gridTemplateRows="repeat(auto-fill, 275px)"
      p={8}
      gap={8}
      overflowY="auto"
    >
      {loading ? (
        <div> Loading organizations... </div>
      ) : (
        <>
          {organizationStore.organizations.map((organization) => (
            <OrganizationCard
              key={organization.id}
              organization={organization}
              onSelect={handleSelect}
            />
          ))}

          <AddOrganizationCard onClick={handleCreate} />
        </>
      )}
    </Box>
  );
});

export default OrganizationSelection;
