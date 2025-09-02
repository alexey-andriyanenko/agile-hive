import React from "react";

import { Dialog, Button } from "@chakra-ui/react";

import type { ModalsPropsBase } from "src/modals-module";

export interface IConfirmModalProps extends ModalsPropsBase {
  title: string;
  onConfirm: () => void | Promise<unknown>;
}

export const ConfirmModal: React.FC<IConfirmModalProps> = ({ title }) => {
  // const [isLoading, setIsLoading] = React.useState(false);
  // const handleConfirm = async () => {
  //   setIsLoading(true);
  //
  //   try {
  //     await onConfirm();
  //     setIsLoading(false);
  //   } finally {
  //     onClose();
  //   }
  // };

  return (
    <Dialog.Root>
      <Dialog.Backdrop />
      <Dialog.Positioner />
      <Dialog.CloseTrigger />

      <Dialog.Content>
        <Dialog.Header>
          <Dialog.Title>{title}</Dialog.Title>
        </Dialog.Header>

        <Dialog.Header></Dialog.Header>
        <Dialog.Footer>
          <Button>Cancel</Button>
          <Button>Confirm</Button>
        </Dialog.Footer>
      </Dialog.Content>
    </Dialog.Root>
  );
};
