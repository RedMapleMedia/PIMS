import React from 'react';
import TooltipWrapper from '../TooltipWrapper';
import { FaPlus } from 'react-icons/fa';
import { Button, ButtonProps } from '.';

interface IPlusButtonProps extends ButtonProps {
  /** set the text of the tooltip that appears on hover of the plus button */
  toolText: string;
  /** set the id of the tool tip use for on hover of the plus buttons */
  toolId: string;
}

/**
 * PlusButton displaying a plus button, used to add new items.
 * @param param0
 */
const PlusButton: React.FC<IPlusButtonProps> = ({ ...props }) => {
  return (
    <TooltipWrapper toolTipId={props.toolId} toolTip={props.toolText}>
      <Button className="bg-success" {...props} icon={<FaPlus size={20} />} />
    </TooltipWrapper>
  );
};

export default PlusButton;
