import React from "react";
import { observer } from "mobx-react-lite";

import { Flex, Button } from "@chakra-ui/react";

import { useModalsStore, useProjectStore, useTagStore } from "src/project-module/store";
import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";
import { TagsList } from "./tags-list";
import { useOrganizationStore } from "src/organization-module/store";

export const ProjectTags: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const tagStore = useTagStore();
  const modalsStore = useModalsStore();
  const sharedModalsStore = useSharedModalsStore();
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    setLoading(true);
    tagStore
      .fetchTagsByProjectId({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: projectStore.currentProject!.id,
      })
      .then(() => setLoading(false))
      .catch((error) => {
        console.error("Failed to fetch tags:", error);
        setLoading(false);
      });
  }, []);

  const handleCreate = async () => {
    modalsStore.open("CreateOrEditTagDialog", {
      onCreate: (data) =>
        tagStore.createTag({
          organizationId: organizationStore.currentOrganization!.id,
          projectId: projectStore.currentProject!.id,
          name: data.name,
          color: data.color,
        }),
    });
  };

  const handleEdit = (tagId: string) => {
    const tag = tagStore.tags.find((t) => t.id === tagId);
    if (!tag) return;

    modalsStore.open("CreateOrEditTagDialog", {
      tag,
      onEdit: (data) =>
        tagStore.updateTag({
          organizationId: organizationStore.currentOrganization!.id,
          projectId: projectStore.currentProject!.id,
          tagId: tag.id,
          name: data.name,
          color: data.color,
        }),
    });
  };

  const handleDelete = (tagId: string) => {
    const tag = tagStore.tags.find((t) => t.id === tagId);
    if (!tag) return;

    sharedModalsStore.open("ConfirmModal", {
      title: "Are you sure you want to delete this tag?",
      description: `Tag: ${tag.name}`,
      onConfirm: () =>
        tagStore.deleteTag(
          organizationStore.currentOrganization!.id,
          tag.id,
          projectStore.currentProject!.id,
        ),
    });
  };

  return (
    <Flex direction="column" width="100%" p={4}>
      {loading ? (
        <div>loading tags...</div>
      ) : (
        <>
          <Flex justify="flex-end" mb={4}>
            <Button variant="outline" onClick={handleCreate}>
              Create Tag
            </Button>
          </Flex>

          <TagsList tags={tagStore.tags} onEdit={handleEdit} onDelete={handleDelete} />
        </>
      )}
    </Flex>
  );
});
